using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示包解析结果
    /// </summary>
    [DebuggerDisplay("Protocol = {Protocol}, DataLength = {DataLength}")]
    public sealed unsafe class WinDivertParseResult
    {
        /// <summary>
        /// ipv4头
        /// </summary>
        public IPV4Header* IPV4Header { get; internal set; }

        /// <summary>
        /// ipv6头
        /// </summary>
        public IPV6Header* IPV6Header { get; internal set; }

        /// <summary>
        /// 包协议
        /// </summary>
        public ProtocolType Protocol { get; internal set; }

        /// <summary>
        /// icmp4头
        /// </summary>
        public IcmpV4Header* IcmpV4Header { get; internal set; }

        /// <summary>
        /// icmp6头
        /// </summary>
        public IcmpV6Header* IcmpV6Header { get; internal set; }

        /// <summary>
        /// tcp头
        /// </summary>
        public TcpHeader* TcpHeader { get; internal set; }

        /// <summary>
        /// udp头
        /// </summary>
        public UdpHeader* UdpHeader { get; internal set; }

        /// <summary>
        /// 负载数据地址
        /// 即经过tcp或udp传输的数据
        /// </summary>
        public byte* Data { get; internal set; }

        /// <summary>
        /// 负载数据长度
        /// </summary>
        public int DataLength { get; internal set; }

        /// <summary>
        /// 负载数据视图
        /// </summary>
        public Span<byte> DataSpan => new(this.Data, this.DataLength);

        /// <summary>
        /// 下一块数据
        /// </summary>
        public byte* Next { get; internal set; }

        /// <summary>
        /// 下一块数据长度
        /// </summary>
        public int NextLength { get; internal set; }

        /// <summary>
        /// 下一块数据视图
        /// </summary>
        public Span<byte> NextSpan => new(this.Next, this.NextLength);
    }
}
