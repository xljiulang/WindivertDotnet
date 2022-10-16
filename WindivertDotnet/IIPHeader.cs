using System.Net;

namespace WindivertDotnet
{
    /// <summary>
    /// IP头接口
    /// </summary>
    public interface IIPHeader
    {
        /// <summary>
        /// 获取或设置版本
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// 获取或设置源地址
        /// </summary>
        public unsafe IPAddress SrcAddr { get; set; }

        /// <summary>
        /// 获取或设置目的地址
        /// </summary>
        public unsafe IPAddress DstAddr { get; set; }
    }
}
