using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// IPV4头
    /// </summary>
    [DebuggerDisplay("SrcAddr = {SrcAddr}, DstAddr = {DstAddr}, Size = {sizeof(IPV4Header)}")]
    public struct IPV4Header
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte bitfield;


        public byte HdrLength
        {
            get => (byte)(bitfield & 0xFu);
            set => bitfield = (byte)((bitfield & ~0xFu) | (value & 0xFu));
        }

        public byte Version
        {
            get => (byte)((bitfield >> 4) & 0xFu);
            set => bitfield = (byte)((bitfield & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        public byte TOS;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort length;

        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort id;

        public ushort Id
        {
            get => BinaryPrimitives.ReverseEndianness(id);
            set => id = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort fragOff0;

        public ushort FragOff0
        {
            get => BinaryPrimitives.ReverseEndianness(fragOff0);
            set => fragOff0 = BinaryPrimitives.ReverseEndianness(value);
        }

        public byte TTL;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte protocol;

        public ProtocolType Protocol => (ProtocolType)protocol;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort checksum;

        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint srcAddr;

        public IPAddress SrcAddr
        {
            get => new(unchecked(this.srcAddr));
            set => this.srcAddr = BinaryPrimitives.ReadUInt32LittleEndian(value.GetAddressBytes());
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint dstAddr;

        public IPAddress DstAddr
        {
            get => new(unchecked(this.dstAddr));
            set => this.dstAddr = BinaryPrimitives.ReadUInt32LittleEndian(value.GetAddressBytes());
        }
    }
}
