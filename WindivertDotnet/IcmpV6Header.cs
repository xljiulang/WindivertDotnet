using System.Buffers.Binary;
using System.Diagnostics;

namespace WindivertDotnet
{
    /// <summary>
    /// IcmpV6头结构
    /// </summary>
    [DebuggerDisplay("Type = {Type}, Code = {Code}, Size = {sizeof(IcmpV6Header)}")]
    public struct IcmpV6Header
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

        /// <summary>
        /// 获取或设置消息体长度
        /// </summary>
        public uint Body;
    }

}
