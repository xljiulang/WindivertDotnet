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
        private readonly WinDivert divert;
        private readonly IdSeqNum idSeqNum = new();

        public FastPinger()
        {
            // 只接受进入系统的icmp
            var filter = Filter.True.And(f => (f.IsIcmp || f.IsIcmpV6) && f.Network.Inbound);
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
        /// 占用两个线程
        /// </summary>
        /// <param name="dstAddrs">目标地址</param>
        /// <param name="delay">最后一个IP发出ping之后的等待回复时长</param>
        /// <returns></returns>
        public async Task<IPAddress[]> PingAllAsync(IEnumerable<IPAddress> dstAddrs, TimeSpan delay)
        {
            // 开始监听ping的回复
            using var cts = new CancellationTokenSource();
            var recvTask = this.RecvEchoReplyAsync(cts.Token);

            // 对所有ip发ping
            await this.SendEchoRequestAsync(dstAddrs);

            // 延时取消监听
            cts.CancelAfter(delay);
            var results = await recvTask;

            // 清洗数据
            return results.Intersect(dstAddrs).ToArray();
        }


        /// <summary>
        /// 监听ping的回复
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        private async Task<HashSet<IPAddress>> RecvEchoReplyAsync(CancellationToken cancellationToken)
        {
            var results = new HashSet<IPAddress>();
            using var packet = new WinDivertPacket();
            using var addr = new WinDivertAddress();

            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    await this.divert.RecvAsync(packet, addr, cancellationToken);
                    if (TryGetEchoReplyAddr(packet, out var value))
                    {
                        results.Add(value);
                    }
                    // 把packet发出，避免系统其它软件此刻也有ping而收不到回复
                    await this.divert.SendAsync(packet, addr, cancellationToken);
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
        /// <param name="packet">数据包</param>
        /// <param name="value">回复的IP</param>
        /// <returns></returns>
        private unsafe static bool TryGetEchoReplyAddr(WinDivertPacket packet, [MaybeNullWhen(false)] out IPAddress value)
        {
            var result = packet.GetParseResult();
            if (result.IcmpV4Header != null &&
                result.IcmpV4Header->Type == IcmpV4MessageType.EchoReply)
            {
                value = result.IPV4Header->SrcAddr;
                return true;
            }
            else if (result.IcmpV6Header != null &&
                result.IcmpV6Header->Type == IcmpV6MessageType.EchoReply)
            {
                value = result.IPV6Header->SrcAddr;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 发送icmp的echo请求包
        /// </summary>
        /// <param name="dstAddrs"></param>
        /// <returns></returns>
        private async Task SendEchoRequestAsync(IEnumerable<IPAddress> dstAddrs)
        {
            foreach (var address in dstAddrs)
            {
                // 使用router计算将进行通讯的本机地址
                var router = new WinDivertRouter(address);
                using var addr = router.CreateAddress();
                using var packet = address.AddressFamily == AddressFamily.InterNetwork
                   ? CreateIPV4EchoPacket(router.SrcAddress, router.DstAddress)
                   : CreateIPV6EchoPacket(router.SrcAddress, router.DstAddress);

                packet.CalcChecksums(addr);     // 计算checksums，因为创建包时没有计算

                await this.divert.SendAsync(packet, addr);
            }
        }

        /// <summary>
        /// 创建icmp的echo包
        /// </summary>
        /// <param name="srcAddr"></param>
        /// <param name="dstAddr"></param>
        /// <returns></returns>
        private unsafe WinDivertPacket CreateIPV4EchoPacket(IPAddress srcAddr, IPAddress dstAddr)
        {
            // ipv4头
            var ipHeader = new IPV4Header
            {
                TTL = 128,
                Version = 4,
                DstAddr = dstAddr,
                SrcAddr = srcAddr,
                Protocol = ProtocolType.Icmp,
                HdrLength = (byte)(sizeof(IPV4Header) / 4),
                Id = IdSeqNum.Shared.NextUInt16(),
                Length = (ushort)(sizeof(IPV4Header) + sizeof(IcmpV4Header))
            };

            // icmp头
            var icmpHeader = new IcmpV4Header
            {
                Type = IcmpV4MessageType.EchoRequest,
                Code = default,
                Identifier = this.idSeqNum.NextUInt16(),
                SequenceNumber = this.idSeqNum.NextUInt16(),
            };

            // 将数据写到packet缓冲区
            var packet = new WinDivertPacket(ipHeader.Length);

            var writer = packet.GetWriter();
            writer.Write(ipHeader);
            writer.Write(icmpHeader);

            return packet;
        }

        /// <summary>
        /// 创建icmpv6的echo包
        /// </summary>
        /// <param name="srcAddr"></param>
        /// <param name="dstAddr"></param>
        /// <returns></returns>
        private unsafe WinDivertPacket CreateIPV6EchoPacket(IPAddress srcAddr, IPAddress dstAddr)
        {
            // ipv6头
            var ipHeader = new IPV6Header
            {
                DstAddr = dstAddr,
                SrcAddr = srcAddr,
                Length = (ushort)(sizeof(IcmpV6Header)),
                Version = 6,
                NextHdr = ProtocolType.IcmpV6,
                HopLimit = 128
            };

            // icmpv6头
            var icmpHeader = new IcmpV6Header
            {
                Type = IcmpV6MessageType.EchoRequest,
                Code = default,
                Identifier = this.idSeqNum.NextUInt16(),
                SequenceNumber = this.idSeqNum.NextUInt16(),
            };

            // 将数据写到packet缓冲区
            var packet = new WinDivertPacket(sizeof(IPV6Header) + ipHeader.Length);

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
