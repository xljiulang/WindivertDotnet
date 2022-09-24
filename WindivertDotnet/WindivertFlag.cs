using System;

namespace WindivertDotnet
{   
    public enum WinDivertFlag : ulong
    {
        None = 0,
        Sniff = 0x0001,
        Drop = 0x0002,
        Recv = 0x0004,
        Read = Recv,
        Send = 0x0008,
        Write = Send,
        NoInstall = 0x0010,
        Fragments = 0x0020,
    }
}
