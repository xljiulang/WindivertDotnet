using System;

namespace WindivertDotnet
{   
    public enum WinDivertFlag : ulong
    {
        None = 0,
        Sniff = 0x0001,
        Drop = 0x0002,
        RecvOnly = 0x0004,
        ReadOnly = RecvOnly,
        SendOnly = 0x0008,
        WriteOnly = SendOnly,
        NoInstall = 0x0010,
        Fragments = 0x0020,
    }
}
