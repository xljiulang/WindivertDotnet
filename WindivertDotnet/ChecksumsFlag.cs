using System;

namespace WindivertDotnet
{
    /// <summary>
    /// 校验和选项
    /// </summary>
    [Flags]
    public enum ChecksumsFlag : ulong
    {
        /// <summary>
        /// 全部
        /// </summary>
        All = 0,

        /// <summary>
        /// 排除IpChecksum
        /// </summary>
        NoIpChecksum = 1,

        /// <summary>
        /// 排除IcmpChecksum
        /// </summary>
        NoIcmpChecksum = 2,

        /// <summary>
        /// 排除IcmpV6Checksum
        /// </summary>
        NoIcmpV6Checksum = 4,

        /// <summary>
        /// 排除TcpChecksum
        /// </summary>
        NoTcpChecksum = 8,

        /// <summary>
        /// 排除UdpChecksum
        /// </summary>
        NoUdpChecksum = 16,
    }
}
