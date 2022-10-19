using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV6头结构
    /// </summary>
    [DebuggerDisplay("Type = {Type}, Code = {Code}, Size = {sizeof(IcmpV6Header)}")]
    [StructLayout(LayoutKind.Explicit)]
    public struct IcmpV6Header
    {
        /// <summary>
        /// 获取或设置类型
        /// </summary>
        [FieldOffset(0)]
        public IcmpV6MessageType Type;

        /// <summary>
        /// 获取或设置代码
        /// </summary>
        [FieldOffset(1)]
        public IcmpV6UnreachableCode Code;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(2)]
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
        [FieldOffset(4)]
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(4)]
        private ushort identifier;

        /// <summary>
        /// 获取或设置id
        /// </summary>
        public ushort Identifier
        {
            get => BinaryPrimitives.ReverseEndianness(identifier);
            set => identifier = BinaryPrimitives.ReverseEndianness(value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(6)]
        private ushort sequenceNumber;

        /// <summary>
        /// 获取或设置序列号
        /// </summary>
        public ushort SequenceNumber
        {
            get => BinaryPrimitives.ReverseEndianness(sequenceNumber);
            set => sequenceNumber = BinaryPrimitives.ReverseEndianness(value);
        }
    }

}
