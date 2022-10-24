using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// Tcp头
    /// </summary>
    [DebuggerDisplay("SrcPort = {SrcPort}, DstPort = {DstPort}, Flags = {Flags}, Size = {HdrLength * 4}")]
    [StructLayout(LayoutKind.Explicit)]
    public struct TcpHeader
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(0)]
        private ushort srcPort;

        /// <summary>
        /// 获取或设置源端口
        /// </summary>
        public ushort SrcPort
        {
            get => BinaryPrimitives.ReverseEndianness(srcPort);
            set => srcPort = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(2)]
        private ushort dstPort;

        /// <summary>
        /// 获取或设置目的端口
        /// </summary>
        public ushort DstPort
        {
            get => BinaryPrimitives.ReverseEndianness(dstPort);
            set => dstPort = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(4)]
        private uint seqNum;

        /// <summary>
        /// 获取或设置序列码
        /// </summary>
        public uint SeqNum
        {
            get => BinaryPrimitives.ReverseEndianness(seqNum);
            set => seqNum = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(8)]
        private uint ackNum;

        /// <summary>
        /// 获取或设置确认码
        /// </summary>
        public uint AckNum
        {
            get => BinaryPrimitives.ReverseEndianness(ackNum);
            set => ackNum = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(12)]
        private ushort bitfield;

        /// <summary>
        /// 标记位
        /// </summary>
        [FieldOffset(13)]
        public TcpFlag Flags;

        /// <summary>
        /// 保留
        /// </summary>
        public ushort Reserved1
        {
            get => (ushort)(bitfield & 0xFu);
            set => bitfield = (ushort)((bitfield & ~0xFu) | (value & 0xFu));
        }

        /// <summary>
        /// 获取或设置Internet Header Length
        /// Tcp头总字节为该值的4倍
        /// </summary>
        public ushort HdrLength
        {
            get => (ushort)((bitfield >> 4) & 0xFu);
            set => bitfield = (ushort)((bitfield & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        /// <summary>
        /// 获取或设置结束位
        /// </summary>
        public ushort Fin
        {
            get => (ushort)((bitfield >> 8) & 0x1u);
            set => bitfield = (ushort)((bitfield & ~(0x1u << 8)) | ((value & 0x1u) << 8));
        }


        /// <summary>
        /// 获取或设置请求位
        /// </summary>
        public ushort Syn
        {
            get => (ushort)((bitfield >> 9) & 0x1u);
            set => bitfield = (ushort)((bitfield & ~(0x1u << 9)) | ((value & 0x1u) << 9));
        }

        /// <summary>
        /// 获取或设置重置位
        /// </summary>
        public ushort Rst
        {
            get => (ushort)((bitfield >> 10) & 0x1u);
            set => bitfield = (ushort)((bitfield & ~(0x1u << 10)) | ((value & 0x1u) << 10));
        }

        /// <summary>
        /// 获取或设置推送位
        /// </summary>
        public ushort Psh
        {
            get => (ushort)((bitfield >> 11) & 0x1u);
            set => bitfield = (ushort)((bitfield & ~(0x1u << 11)) | ((value & 0x1u) << 11));
        }

        /// <summary>
        /// 获取或设置确认位
        /// </summary>
        public ushort Ack
        {
            get => (ushort)((bitfield >> 12) & 0x1u);
            set => bitfield = (ushort)((bitfield & ~(0x1u << 12)) | ((value & 0x1u) << 12));
        }

        /// <summary>
        /// 获取或设置紧急位
        /// </summary>
        public ushort Urg
        {
            get => (ushort)((bitfield >> 13) & 0x1u);
            set => bitfield = (ushort)((bitfield & ~(0x1u << 13)) | ((value & 0x1u) << 13));
        }

        /// <summary>
        /// 保留
        /// </summary>
        public ushort Reserved2
        {
            get => (ushort)((bitfield >> 14) & 0x3u);
            set => bitfield = (ushort)((bitfield & ~(0x3u << 14)) | ((value & 0x3u) << 14));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(14)]
        private ushort window;

        /// <summary>
        /// 获取或设置滑动窗口
        /// </summary>
        public ushort Window
        {
            get => BinaryPrimitives.ReverseEndianness(window);
            set => window = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(16)]
        private ushort checksum;

        /// <summary>
        /// 获取或设置校验和
        /// </summary>
        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(18)]
        private ushort urgPtr;

        /// <summary>
        /// 获取或设置紧急指针
        /// </summary>
        public ushort UrgPtr
        {
            get => BinaryPrimitives.ReverseEndianness(urgPtr);
            set => urgPtr = BinaryPrimitives.ReverseEndianness(value);
        }
    }
}
