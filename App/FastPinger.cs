using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WindivertDotnet;

namespace App
{
    class FastPinger : IDisposable
    {
        private ushort id = 0;
        private ushort sequenceNumber = 0;
        private readonly WinDivert divert;

        public FastPinger()
        {
            // 只接受进入系统的icmp
            var filter = Filter.True.And(f => f.IsIcmp && f.Network.Inbound);
            this.divert = new WinDivert(filter, WinDivertLayer.Network);
        }

        /// <summary>
        /// Ping所有地址
        /// 占用两个线程
        /// </summary>
        /// <param name="startAddr">开始地址</param>
        /// <param name="count">IP数量</param>
        /// <param name="delay">最后一个IP发出ping之后的等待回复时长</param>
        /// <returns></returns>
        public Task<IPAddress[]> PingAllAsync(IPAddress startAddr, int count, TimeSpan delay)
        {
            var dstAddrs = CreateAddrs(startAddr, count);
            return this.PingAllAsync(dstAddrs, delay);
        }

        /// <summary>
        /// 创建IP列表
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IEnumerable<IPAddress> CreateAddrs(IPAddress startAddr, int count)
        {
            var start = BinaryPrimitives.ReadUInt32LittleEndian(startAddr.GetAddressBytes());
            for (var i = 0; i < count; i++)
            {
                var value = BinaryPrimitives.ReverseEndianness((uint)(start + i));
                yield return new IPAddress(value);
            }
        }

        /// <summary>
        /// Ping所有地址
        /// 占用两个线程
        /// </summary>
        /// <param name="dstAddrs">目标地址</param>
        /// <param name="delay">最后一个IP发出ping之后的等待回复时长</param>
        /// <returns></returns>
        public async Task<IPAddress[]> PingAllAsync(IEnumerable<IPAddress> dstAddrs, TimeSpan delay)
        {
            var hashSet = new HashSet<IPAddress>();
            using var cts = new CancellationTokenSource();

            // 开始监听ping的回复
            var recvTask = this.RecvReplyAsync(hashSet, cts.Token);

            // 对所有ip发ping
            await this.SendEchoAsync(dstAddrs);

            // 取消监听
            cts.CancelAfter(delay);

            try
            {
                await recvTask;
            }
            catch (OperationCanceledException)
            {
            }

            // 清洗数据
            return hashSet.Intersect(dstAddrs).ToArray();
        }


        /// <summary>
        /// 监听ping的回复
        /// </summary>
        /// <param name="replyHashSet">回复的IP列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        private async Task RecvReplyAsync(HashSet<IPAddress> replyHashSet, CancellationToken cancellationToken)
        {
            using var packet = new WinDivertPacket();
            using var addr = new WinDivertAddress();

            while (cancellationToken.IsCancellationRequested == false)
            {
                await this.divert.RecvAsync(packet, addr, cancellationToken);
                if (TryGetReply(packet, out var replyAddr))
                {
                    replyHashSet.Add(replyAddr);
                }

                // 把packet发出，避免系统其它软件此刻也有ping而收不到回复
                await this.divert.SendAsync(packet, addr, cancellationToken);
            }
        }


        /// <summary>
        /// 解析出icmp回复信息
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="srcAddr">回复的IP</param>
        /// <returns></returns>
        private unsafe static bool TryGetReply(WinDivertPacket packet, [MaybeNullWhen(false)] out IPAddress srcAddr)
        {
            var result = packet.GetParseResult();
            if (result.IcmpV4Header != null && result.IcmpV4Header->Code == 0) // 0是icmp的回复
            {
                srcAddr = result.IPV4Header->SrcAddr;
                return true;
            }

            srcAddr = null;
            return false;
        }

        /// <summary>
        /// 发送icmp的echo命令
        /// </summary>
        /// <param name="dstAddrs"></param>
        /// <returns></returns>
        private async Task SendEchoAsync(IEnumerable<IPAddress> dstAddrs)
        {
            foreach (var address in dstAddrs)
            {
                // 使用router计算将进行通讯的本机地址
                var router = new WinDivertRouter(address);
                using var packet = this.CreateEchoPacket(router);
                using var addr = new WinDivertAddress();

                packet.CalcChecksums(addr);     // 计算checksums
                packet.CalcLoopbackFlag(addr);  // 计算loopback(实际不算也知道不是Loopback)
                packet.CalcNetworkIfIdx(addr);  // 计算网卡索引
                packet.CalcOutboundFlag(addr);  // 计算是否为出口(实际不算也知道是Outbound)

                await this.divert.SendAsync(packet, addr);
            }
        }

        /// <summary>
        /// 创建icmp的echo包
        /// </summary>
        /// <param name="router"></param>
        /// <returns></returns>
        private unsafe WinDivertPacket CreateEchoPacket(WinDivertRouter router)
        {
            // ipv4头
            var ipHeader = new IPV4Header
            {
                TTL = 128,
                Version = 4,
                DstAddr = router.DstAddress,
                SrcAddr = router.SrcAddress,
                Protocol = ProtocolType.Icmp,
                HdrLength = (byte)(sizeof(IPV4Header) / 4),
                Id = this.id++,
                Length = (ushort)(sizeof(IPV4Header) + sizeof(IcmpV4Header))
            };

            // icmp头
            var icmpHeader = new IcmpV4Header
            {
                Type = 8, // echo
                Code = 0,
                Identifier = ipHeader.Id,
                SequenceNumber = this.sequenceNumber++,
            };

            // 将数据写到packet缓冲区
            var packet = new WinDivertPacket(ipHeader.Length);

            var writer = packet.GetWriter();
            writer.Write(ipHeader);
            writer.Write(icmpHeader);

            return packet;
        }

        public void Dispose()
        {
            this.divert.Dispose();
        }
    }
}
