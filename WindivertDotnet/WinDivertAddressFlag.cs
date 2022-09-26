using System;

namespace WindivertDotnet
{
    [Flags]
    public enum WinDivertAddressFlag : byte
    {
        None = 0,
        Sniffed = 0x1,
        Outbound = 0x2,
        Loopback = 0x4,
        Impostor = 0x8,
        IPv6 = 0x10,
        IPChecksum = 0x20,
        TCPChecksum = 0x40,
        UDPChecksum = 0x80
    }
}
