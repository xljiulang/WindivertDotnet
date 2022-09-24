using System;

namespace WindivertDotnet
{
    [Flags]
    public enum WINDIVERT_FLAG : ulong
    {
        SNIFF = 0x0001,
        DROP = 0x0002,
        RECV_ONLY = 0x0004,
        READ_ONLY = RECV_ONLY,
        SEND_ONLY = 0x0008,
        WRITE_ONLY = SEND_ONLY,
        NO_INSTALL = 0x0010,
        FRAGMENTS = 0x0020,
    }
}
