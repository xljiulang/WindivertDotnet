using System;

namespace WindivertDotnet
{
    [Flags]
    public enum WinDivertShutdown
    {
        Recv = 0x1,
        Send = 0x2,
        Both = Recv | Send
    }
}
