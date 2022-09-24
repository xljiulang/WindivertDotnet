using System;
using System.Net.Sockets;

namespace WindivertDotnet
{
    public unsafe class WinDivertParseResult
    {
        public IPV4Header* IPV4Header { get; init; }
        public IPV6Header* IPV6Header { get; init; }
        public ProtocolType Protocol { get; init; }
        public IcmpV4Header* IcmpV4Header { get; init; }
        public IcmpV6Header* IcmpV6Header { get; init; }
        public TcpHeader* TcpHeader { get; init; }
        public UdpHeader* UdpHeader { get; init; }
        public byte* Data { get; init; }
        public int DataLength { get; init; }
        public Span<byte> DataSpan => new(this.Data, this.DataLength);

        public byte* Next { get; init; }
        public int NextLength { get; init; }
        public Span<byte> NextSpan => new(this.Next, this.NextLength);
    }
}
