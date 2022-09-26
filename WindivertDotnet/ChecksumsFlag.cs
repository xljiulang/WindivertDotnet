using System;

namespace WindivertDotnet
{
    /// <summary>
    /// 校验和选项
    /// </summary>
    [Flags]
    public enum ChecksumsFlag : ulong
    {
        All = 0,
        NoIpChecksum = 1,
        NoIcmpChecksum = 2,
        NoIcmpV6Checksum = 4,
        NoTcpChecksum = 8,
        NoUdpChecksum = 16,
    }
}
