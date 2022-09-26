using System.Buffers.Binary;
using System.Diagnostics;

namespace WindivertDotnet
{
    /// <summary>
    /// Udp头
    /// </summary>
    [DebuggerDisplay("SrcPort = {SrcPort}, DstPort = {DstPort}, Size = {sizeof(UdpHeader)}")]
    public struct UdpHeader
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
        private ushort length;

        /// <summary>
        /// 获取或设置Udp包长度
        /// [含头部的8字节]
        /// </summary>
        public ushort Length
        {
            get => BinaryPrimitives.ReverseEndianness(length);
            set => length = BinaryPrimitives.ReverseEndianness(value);
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort checksum;


        /// <summary>
        /// 获取或设置校验和
        /// </summary>
        public ushort Checksum
        {
            get => BinaryPrimitives.ReverseEndianness(checksum);
            set => checksum = BinaryPrimitives.ReverseEndianness(value);
        }
    }

}
