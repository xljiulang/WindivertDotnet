using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WindivertDotnet;

namespace App.Icmp
{

    /// <summary>
    /// 快速ping工具
    /// </summary>
    static class FastPinger
    {
        private record SrcAddrSeqNum(IPAddress SrcAddress, ushort SeqNum);

        /// <summary>
        /// Ping所有地址 
        /// </summary>
        /// <param name="startAddr">开始地址</param>
        /// <param name="count">IP数量</param>
        /// <param name="waitTime">最后一个IP发出ping之后的等待回复时长</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<IPAddress[]> PingAllAsync(IPAddress startAddr, int count, TimeSpan waitTime, CancellationToken cancellationToken = default)
        {
            var dstAddrs = CreateAddrs(startAddr, count);
            return PingAllAsync(dstAddrs, waitTime, cancellationToken);
        }

        /// <summary>
        /// 创建IP列表
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IEnumerable<IPAddress> CreateAddrs(IPAddress startAddr, int count)
        {
            var bytes = startAddr.GetAddressBytes();
            var start = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(bytes.Length - 4));

            for (var i = 0; i < count; i++)
            {
                var value = (uint)(start + i);
                BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(bytes.Length - 4), value);
                yield return new IPAddress(bytes);
            }
        }

        /// <summary>
        /// Ping所有地址 
        /// </summary>
        /// <param name="dstAddrs">目标地址</param>
        /// <param name="waitTime">最后一个IP发出ping之后的等待回复时长</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<IPAddress[]> PingAllAsync(IEnumerable<IPAddress> dstAddrs, TimeSpan waitTime, CancellationToken cancellationToken = default)
        {
            var filter = Filter.True
                .And(f => f.Network.Inbound)
                .And(f => f.ICmp.Type == IcmpV4MessageType.EchoReply || f.IcmpV6.Type == IcmpV6MessageType.EchoReply);

            using var divert = new WinDivert(filter, WinDivertLayer.Network);

            // 开始监听ping的回复
            using var waitTokenSource = new CancellationTokenSource();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(waitTokenSource.Token, cancellationToken);
            var recvTask = RecvRepliesAsync(divert, linkedTokenSource.Token);
            var seqNums = await SendPingsAsync(divert, dstAddrs, cancellationToken);

            // 延时取消监听
            waitTokenSource.CancelAfter(waitTime);
            var results = await recvTask;

            return results
                .Where(item => seqNums.Contains(item.SeqNum))
                .Select(item => item.SrcAddress)
                .Distinct()
                .ToArray();
        }


        /// <summary>
        /// 接收回复信息
        /// </summary>
        /// <param name="divert"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async static Task<List<SrcAddrSeqNum>> RecvRepliesAsync(WinDivert divert, CancellationToken cancellationToken)
        {
            var results = new List<SrcAddrSeqNum>();
            using var packet = new WinDivertPacket();
            using var addr = new WinDivertAddress();

            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    await divert.RecvAsync(packet, addr, cancellationToken);

                    if (TryParseReply(packet, out var srcAddrSeqNum))
                    {
                        results.Add(srcAddrSeqNum);
                    }

                    await divert.SendAsync(packet, addr, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            return results;
        }


        /// <summary>
        /// 解析出icmp回复信息
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="srcAddrSeqNum"></param>
        /// <returns></returns>
        private unsafe static bool TryParseReply(WinDivertPacket packet, [MaybeNullWhen(false)] out SrcAddrSeqNum srcAddrSeqNum)
        {
            var result = packet.GetParseResult();
            var srcAddr = result.IPV4Header != null
                ? result.IPV4Header->SrcAddr
                : result.IPV6Header->SrcAddr;

            if (result.IcmpV4Header != null)
            {
                srcAddrSeqNum = new SrcAddrSeqNum(srcAddr, result.IcmpV4Header->SequenceNumber);
                return true;
            }

            if (result.IcmpV6Header != null)
            {
                srcAddrSeqNum = new SrcAddrSeqNum(srcAddr, result.IcmpV6Header->SequenceNumber);
                return true;
            }

            srcAddrSeqNum = null;
            return false;
        }

        /// <summary>
        /// 发送ping请求
        /// </summary>
        /// <param name="divert"></param>
        /// <param name="dstAddrs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<HashSet<ushort>> SendPingsAsync(WinDivert divert, IEnumerable<IPAddress> dstAddrs, CancellationToken cancellationToken)
        {
            var seqNums = new HashSet<ushort>();
            foreach (var address in dstAddrs)
            {
                // 使用router计算将进行通讯的本机地址
                var router = new WinDivertRouter(address);
                using var addr = router.CreateAddress();

                using var packet = new PingPacket(router);
                packet.CalcChecksums(addr);
                await divert.SendAsync(packet, addr, cancellationToken);
                seqNums.Add(packet.SeqNum);
            }
            return seqNums;
        }
    }
}
