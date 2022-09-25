using System;
using System.Net.Sockets;

namespace WindivertDotnet
{
    public unsafe class WinDivertParseResult
    {
        public IPV4Header* IPV4Header { get; internal set; }
        public IPV6Header* IPV6Header { get; internal set; }
        public ProtocolType Protocol { get; internal set; }
        public IcmpV4Header* IcmpV4Header { get; internal set; }
        public IcmpV6Header* IcmpV6Header { get; internal set; }
        public TcpHeader* TcpHeader { get; internal set; }
        public UdpHeader* UdpHeader { get; internal set; }
        public byte* Data { get; internal set; }
        public int DataLength { get; internal set; }
        public Span<byte> DataSpan => new(this.Data, this.DataLength);

        public byte* Next { get; internal set; }
        public int NextLength { get; internal set; }
        public Span<byte> NextSpan => new(this.Next, this.NextLength);
    }
}
