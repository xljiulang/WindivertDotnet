using System.Net.Sockets;
using WindivertDotnet;

namespace App.Icmp
{
    /// <summary>
    /// ping包
    /// </summary>
    unsafe class PingPacket : WinDivertPacket
    {
        private static readonly IdSeqNum id = new();
        private static readonly IdSeqNum seqNum = new();
        private static readonly ushort v4PacketLength = (ushort)(sizeof(IPV4Header) + sizeof(IcmpV4Header) + 32);
        private static readonly ushort v6PacketLength = (ushort)(sizeof(IPV6Header) + sizeof(IcmpV6Header) + 32);

        public ushort Id { get; } = id.NextUInt16();

        public ushort SeqNum { get; } = seqNum.NextUInt16();

        /// <summary>
        /// ping包
        /// </summary>
        /// <param name="router"></param>
        /// <param name="ttl"></param>
        public PingPacket(WinDivertRouter router, byte ttl = 128)
            : base(v6PacketLength)
        {
            if (router.DstAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var ipHeader = new IPV4Header
                {
                    TTL = ttl,
                    Version = IPVersion.V4,
                    DstAddr = router.DstAddress,
                    SrcAddr = router.SrcAddress,
                    Protocol = ProtocolType.Icmp,
                    HdrLength = 5,
                    Id = IdSeqNum.Shared.NextUInt16(),
                    Length = v4PacketLength
                };

                var icmpHeader = new IcmpV4Header
                {
                    Type = IcmpV4MessageType.EchoRequest,
                    Identifier = Id,
                    SequenceNumber = SeqNum,
                };

                var writer = GetWriter();
                writer.Write(ipHeader);
                writer.Write(icmpHeader);
                writer.Advance(32);
            }
            else
            {
                var ipHeader = new IPV6Header
                {
                    SrcAddr = router.SrcAddress,
                    DstAddr = router.DstAddress,
                    Length = v6PacketLength,
                    HopLimit = ttl,
                    NextHdr = ProtocolType.IcmpV6,
                    Version = IPVersion.V6
                };
                var icmpHeader = new IcmpV6Header
                {
                    Type = IcmpV6MessageType.EchoRequest,
                    Identifier = Id,
                    SequenceNumber = SeqNum
                };

                var writer = GetWriter();
                writer.Write(ipHeader);
                writer.Write(icmpHeader);
                writer.Advance(32);
            }
        }

    }
}
