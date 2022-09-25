using System;

namespace WindivertDotnet
{
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
