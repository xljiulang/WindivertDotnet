using System.Buffers.Binary;
using System.Diagnostics;

namespace WindivertDotnet
{
    [DebuggerDisplay("Type = {Type}, Code = {Code}")]
    public struct IcmpV6Header
    {
        public byte Type;

        public byte Code;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort checksum;

        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }

        public uint Body;
    }

}
