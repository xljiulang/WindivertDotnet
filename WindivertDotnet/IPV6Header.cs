using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;

namespace WindivertDotnet
{
    [DebuggerDisplay("SrcAddr = {SrcAddr}, DstAddr = {DstAddr}")]
    public struct IPV6Header
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public byte bitfield1;


        public byte TrafficClass0
        {
            get => (byte)(bitfield1 & 0xFu);
            set => bitfield1 = (byte)((bitfield1 & ~0xFu) | (value & 0xFu));
        }

        public byte Version
        {
            get => (byte)((bitfield1 >> 4) & 0xFu);
            set => bitfield1 = (byte)((bitfield1 & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte bitfield2;


        public byte FlowLabel0
        {
            get => (byte)(bitfield2 & 0xFu);
            set => bitfield2 = (byte)((bitfield2 & ~0xFu) | (value & 0xFu));
        }


        public byte TrafficClass1
        {
            get => (byte)((bitfield2 >> 4) & 0xFu);
            set => bitfield2 = (byte)((bitfield2 & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort flowLabel1;

        public ushort FlowLabel1
        {
            get => BinaryPrimitives.ReverseEndianness(flowLabel1);
            set => flowLabel1 = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort length;

        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }


        public byte NextHdr;

        public byte HopLimit;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint srcAddrA;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint srcAddrB;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint srcAddrC;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint srcAddrD;

        public IPAddress SrcAddr
        {
            get
            {
                var bytes = new byte[16];
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 0), srcAddrA);
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 1), srcAddrB);
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 2), srcAddrC);
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 3), srcAddrD);
                return new(bytes);
            }
            set
            {
                var bytes = value.GetAddressBytes();
                srcAddrA = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 0));
                srcAddrB = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 1));
                srcAddrC = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 2));
                srcAddrD = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 3));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint dstAddrA;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint dstAddrB;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint dstAddrC;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint dstAddrD;

        public IPAddress DstAddr
        {
            get
            {
                var bytes = new byte[16];
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 0), dstAddrA);
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 1), dstAddrB);
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 2), dstAddrC);
                BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 3), dstAddrD);
                return new(bytes);
            }
            set
            {
                var bytes = value.GetAddressBytes();
                dstAddrA = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 0));
                dstAddrB = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 1));
                dstAddrC = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 2));
                dstAddrD = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(sizeof(uint) * 3));
            }
        }
    }
}
