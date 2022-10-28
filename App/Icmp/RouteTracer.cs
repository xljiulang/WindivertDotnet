using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using WindivertDotnet;

namespace App.Icmp
{
    /// <summary>
    /// 路由信息
    /// </summary>
    static class RouteTracer
    {
        private record SrcAddrSeqNum(IPAddress SrcAddress, ushort SeqNum);

        /// <summary>
        /// 测试路由
        /// </summary>
        /// <param name="dstAddr"></param>
        /// <param name="waitTime"></param>
        /// <param name="ttlCount"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="NetworkInformationException"></exception>
        /// <returns></returns>
        public async static Task<IPAddress[]> TraceAsync(IPAddress dstAddr, byte ttlCount, TimeSpan waitTime, CancellationToken cancellationToken = default)
        {
            var filter = Filter.True
                .And(f => f.Network.Inbound)
                .And(f => f.ICmp.Type == IcmpV4MessageType.TimeExceeded || f.IcmpV6.Type == IcmpV6MessageType.TimeExceeded);

            using var divert = new WinDivert(filter, WinDivertLayer.Network);

            // 开始监听ping的回复
            using var waitTokenSource = new CancellationTokenSource();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(waitTokenSource.Token, cancellationToken);
            var recvTask = RecvRepliesAsync(divert, linkedTokenSource.Token);

            // 开始ttl发ping
            var ttls = Enumerable.Range(1, ttlCount).Select(i => (byte)i);
            var seqNums = await SendPingsAsync(divert, dstAddr, ttls, cancellationToken);

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
            if (result.DataLength > 0)
            {
                var srcAddr = result.IPV4Header != null
                    ? result.IPV4Header->SrcAddr
                    : result.IPV6Header->SrcAddr;

                var dataPacket = packet.Slice(packet.Length - result.DataLength, result.DataLength);
                dataPacket.ApplyLengthToHeaders();

                var dataResult = dataPacket.GetParseResult();
                if (dataResult.IcmpV4Header != null)
                {
                    srcAddrSeqNum = new SrcAddrSeqNum(srcAddr, dataResult.IcmpV4Header->SequenceNumber);
                    return true;
                }
                if (dataResult.IcmpV6Header != null)
                {
                    srcAddrSeqNum = new SrcAddrSeqNum(srcAddr, dataResult.IcmpV6Header->SequenceNumber);
                    return true;
                }
            }

            srcAddrSeqNum = null;
            return false;
        }

        /// <summary>
        /// 发送ping请求包
        /// </summary>
        /// <param name="divert"></param>
        /// <param name="dstAddr"></param>
        /// <param name="ttls"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async static Task<HashSet<ushort>> SendPingsAsync(WinDivert divert, IPAddress dstAddr, IEnumerable<byte> ttls, CancellationToken cancellationToken)
        {
            var router = new WinDivertRouter(dstAddr);
            using var addr = router.CreateAddress();
            var result = new HashSet<ushort>();
            foreach (var ttl in ttls)
            {
                using var packet = new PingPacket(router, ttl);
                packet.CalcChecksums(addr);
                await divert.SendAsync(packet, addr, cancellationToken);
                result.Add(packet.SeqNum);
            }
            return result;
        }
    }
}
