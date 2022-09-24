using System.Buffers.Binary;
using System.ComponentModel;

namespace WindivertDotnet
{
    public struct IcmpV4Header
    {
        public byte Type;

        public byte Code;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private ushort checksum;

        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }

        public uint Body;
    }
}
