using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;

namespace WindivertDotnet
{
    /// <summary>
    /// IPv6头
    /// </summary>
    [DebuggerDisplay("SrcAddr = {SrcAddr}, DstAddr = {DstAddr}, Size = {sizeof(IPV6Header)}")]
    public struct IPV6Header
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte bitfield1;

        /// <summary>
        /// 获取或设置流量类
        /// </summary>
        public byte TrafficClass0
        {
            get => (byte)(bitfield1 & 0xFu);
            set => bitfield1 = (byte)((bitfield1 & ~0xFu) | (value & 0xFu));
        }

        /// <summary>
        /// 获取或设置版本
        /// </summary>
        public byte Version
        {
            get => (byte)((bitfield1 >> 4) & 0xFu);
            set => bitfield1 = (byte)((bitfield1 & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte bitfield2;

        /// <summary>
        /// 获取或设置流标签
        /// </summary>
        public byte FlowLabel0
        {
            get => (byte)(bitfield2 & 0xFu);
            set => bitfield2 = (byte)((bitfield2 & ~0xFu) | (value & 0xFu));
        }


        /// <summary>
        /// 获取或设置流量类
        /// </summary>
        public byte TrafficClass1
        {
            get => (byte)((bitfield2 >> 4) & 0xFu);
            set => bitfield2 = (byte)((bitfield2 & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort flowLabel1;

        /// <summary>
        /// 获取或设置流标签
        /// </summary>
        public ushort FlowLabel1
        {
            get => BinaryPrimitives.ReverseEndianness(flowLabel1);
            set => flowLabel1 = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort length;

        /// <summary>
        /// 获取或设置有效负载长度
        /// </summary>
        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// 获取或设置下一个报头
        /// </summary>
        public byte NextHdr;

        /// <summary>
        /// 获取或设置跳跃限制
        /// </summary>
        public byte HopLimit;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private unsafe fixed byte srcAddr[sizeof(int) * 4];

        /// <summary>
        /// 获取或设置源IPv6
        /// </summary>
        public unsafe IPAddress SrcAddr
        {
            get
            {
                fixed (void* ptr = this.srcAddr)
                {
                    return new(new Span<byte>(ptr, sizeof(int) * 4));
                }
            }
            set
            {
                fixed (void* ptr = this.srcAddr)
                {
                    var bytes = value.GetAddressBytes();
                    bytes.CopyTo(new Span<byte>(ptr, sizeof(int) * 4));
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private unsafe fixed byte dstAddr[sizeof(int) * 4];

        /// <summary>
        /// 获取或设置目的IPV6
        /// </summary>
        public unsafe IPAddress DstAddr
        {
            get
            {
                fixed (void* ptr = this.dstAddr)
                {
                    return new(new Span<byte>(ptr, sizeof(int) * 4));
                }
            }
            set
            {
                fixed (void* ptr = this.dstAddr)
                {
                    var bytes = value.GetAddressBytes();
                    bytes.CopyTo(new Span<byte>(ptr, sizeof(int) * 4));
                }
            }
        }
    }
}
