using System.Buffers.Binary;
using System.Diagnostics;

namespace WindivertDotnet
{
    [DebuggerDisplay("SrcPort = {SrcPort}, DstPort = {DstPort}")]
    public struct UdpHeader
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort srcPort;

        public ushort SrcPort
        {
            get => BinaryPrimitives.ReverseEndianness(srcPort);
            set => srcPort = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort dstPort;

        public ushort DstPort
        {
            get => BinaryPrimitives.ReverseEndianness(dstPort);
            set => dstPort = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort length;


        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort checksum;

        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }
    }

}
