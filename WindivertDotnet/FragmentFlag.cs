using System;

namespace WindivertDotnet
{
    /// <summary>
    /// IPv4标记位
    /// </summary>
    [Flags]
    public enum FragmentFlag : byte
    { 
        /// <summary>
        /// 有分片
        /// </summary>
        MoreFragments = 0x1,

        /// <summary>
        /// 无分片
        /// </summary>
        DontFragment = 0x02,

        /// <summary>
        /// 保护
        /// </summary>
        Reserved = 0x04,
    }
}
