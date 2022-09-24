using System.Buffers.Binary;
using System.ComponentModel;

namespace WindivertDotnet
{
    public struct UdpHeader
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        private ushort srcPort;

        /// <summary>
        /// Gets or sets the source port.
        /// </summary>
        public ushort SrcPort
        {
            get => BinaryPrimitives.ReverseEndianness(srcPort);
            set => srcPort = BinaryPrimitives.ReverseEndianness(value);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        private ushort dstPort;

        /// <summary>
        /// Gets or sets the destination port.
        /// </summary>
        public ushort DstPort
        {
            get => BinaryPrimitives.ReverseEndianness(dstPort);
            set => dstPort = BinaryPrimitives.ReverseEndianness(value);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        private ushort length;

        /// <summary>
        /// Gets or sets the header length.
        /// </summary>
        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        private ushort checksum;

        /// <summary>
        /// Gets or sets the checksum.
        /// </summary>
        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }
    }

}
