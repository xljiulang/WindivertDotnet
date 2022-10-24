using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// Tcp头
    /// </summary>
    [DebuggerDisplay("SrcPort = {SrcPort}, DstPort = {DstPort}, Flags = {Flags}, Size = {HdrLength * 4}")]
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
        private byte bitfield;


        /// <summary>
        /// 保留
        /// </summary>
        public byte Reserved1
        {
            get => (byte)(bitfield & 0xFu);
            set => bitfield = (byte)((bitfield & ~0xFu) | (value & 0xFu));
        }

        /// <summary>
        /// 获取或设置Internet Header Length
        /// Tcp头总字节为该值的4倍
        /// </summary>
        public byte HdrLength
        {
            get => (byte)((bitfield >> 4) & 0xFu);
            set => bitfield = (byte)((bitfield & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        /// <summary>
        /// 标记位
        /// </summary>
        [FieldOffset(13)]
        public TcpFlag Flags;


        /// <summary>
        /// 获取或设置结束位
        /// </summary>
        public bool Fin
        {
            get => this.Flags.HasFlag(TcpFlag.Fin);
            set => this.Flags |= TcpFlag.Fin;
        }

        /// <summary>
        /// 获取或设置请求位
        /// </summary>
        public bool Syn
        {
            get => this.Flags.HasFlag(TcpFlag.Syn);
            set => this.Flags |= TcpFlag.Syn;
        }

        /// <summary>
        /// 获取或设置重置位
        /// </summary>
        public bool Rst
        {
            get => this.Flags.HasFlag(TcpFlag.Rst);
            set => this.Flags |= TcpFlag.Rst;
        }

        /// <summary>
        /// 获取或设置推送位
        /// </summary>
        public bool Psh
        {
            get => this.Flags.HasFlag(TcpFlag.Psh);
            set => this.Flags |= TcpFlag.Psh;
        }

        /// <summary>
        /// 获取或设置确认位
        /// </summary>
        public bool Ack
        {
            get => this.Flags.HasFlag(TcpFlag.Ack);
            set => this.Flags |= TcpFlag.Ack;
        }

        /// <summary>
        /// 获取或设置紧急位
        /// </summary>
        public bool Urg
        {
            get => this.Flags.HasFlag(TcpFlag.Urg);
            set => this.Flags |= TcpFlag.Urg;
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
