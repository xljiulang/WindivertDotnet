using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// IPV4头
    /// </summary>
    [DebuggerDisplay("SrcAddr = {SrcAddr}, DstAddr = {DstAddr}, Size = {HdrLength * 4}")]
    public struct IPV4Header
    {
        private const int IPV4_SIZE = sizeof(int);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte bitfield;

        /// <summary>
        /// 获取或设置Internet Header Length
        /// ipv4头总字节为该值的4倍
        /// </summary>
        public byte HdrLength
        {
            get => (byte)(bitfield & 0xFu);
            set => bitfield = (byte)((bitfield & ~0xFu) | (value & 0xFu));
        }

        /// <summary>
        /// 获取或设置版本
        /// </summary>
        public byte Version
        {
            get => (byte)((bitfield >> 4) & 0xFu);
            set => bitfield = (byte)((bitfield & ~(0xFu << 4)) | ((value & 0xFu) << 4));
        }

        /// <summary>
        /// 获取或设置服务类型
        /// </summary>
        public byte TOS;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort length;

        /// <summary>
        /// 获取或设置IP数据包的长度
        /// 包含IPv4头的长度
        /// </summary>
        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort id;

        /// <summary>
        /// 获取或设置报文的id
        /// <para>其初始值由系统随机生成；每发送一个数据报，其值就加1</para>
        /// <para>该值在数据报分片时被复制到每个分片中，因此同一个数据报的所有分片都具有相同的标识值</para>
        /// </summary>
        public ushort Id
        {
            get => BinaryPrimitives.ReverseEndianness(id);
            set => id = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort fragOff0;

        /// <summary>
        /// 获取或设置分片偏移
        /// </summary>
        public ushort FragOff0
        {
            get => BinaryPrimitives.ReverseEndianness(fragOff0);
            set => fragOff0 = BinaryPrimitives.ReverseEndianness(value);
        }

        /// <summary>
        /// 获取或设置生存时间
        /// </summary>
        public byte TTL;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte protocol;


        /// <summary>
        /// 获取或设置协议类型
        /// </summary>
        public ProtocolType Protocol
        {
            get => (ProtocolType)protocol;
            set => protocol = (byte)value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort checksum;

        /// <summary>
        /// 获取或设置检验和
        /// </summary>
        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private unsafe fixed byte srcAddr[IPV4_SIZE];

        /// <summary>
        /// 获取或设置源IPv4
        /// </summary>
        public unsafe IPAddress SrcAddr
        {
            get
            {
                fixed (void* ptr = this.srcAddr)
                {
                    return new(new Span<byte>(ptr, IPV4_SIZE));
                }
            }
            set
            {
                fixed (void* ptr = this.srcAddr)
                {
                    var bytes = value.GetAddressBytes();
                    bytes.CopyTo(new Span<byte>(ptr, IPV4_SIZE));
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private unsafe fixed byte dstAddr[IPV4_SIZE];

        /// <summary>
        /// 获取或设置目的IPv4
        /// </summary>
        public unsafe IPAddress DstAddr
        {
            get
            {
                fixed (void* ptr = this.dstAddr)
                {
                    return new(new Span<byte>(ptr, IPV4_SIZE));
                }
            }
            set
            {
                fixed (void* ptr = this.dstAddr)
                {
                    var span = value.GetAddressBytes().AsSpan();
                    span.CopyTo(new Span<byte>(ptr, IPV4_SIZE));
                }
            }
        }
    }
}
