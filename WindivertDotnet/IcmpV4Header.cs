using System.Buffers.Binary;
using System.Diagnostics;

namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV4头结构
    /// </summary>
    [DebuggerDisplay("Type = {Type}, Code = {Code}, Size = {sizeof(IcmpV4Header)}")]
    public struct IcmpV4Header
    {
        /// <summary>
        /// 获取或设置类型
        /// </summary>
        public byte Type;

        /// <summary>
        /// 获取或设置代码
        /// </summary>
        public byte Code;

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
        private uint body;

        /// <summary>
        /// 获取或设置Rest of header
        /// 内容因 ICMP 类型和代码而异
        /// </summary>
        public uint Body
        {
            get => BinaryPrimitives.ReverseEndianness(body);
            set => body = BinaryPrimitives.ReverseEndianness(value);
        }
    }
}
