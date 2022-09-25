using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct WinDivertAddress
    {
        [FieldOffset(0)]
        public long Timestamp;

        [FieldOffset(8)]
        public WinDivertLayer Layer;

        [FieldOffset(9)]
        public WinDivertEvent Event;

        [FieldOffset(10)]
        public WinDivertAddressFlag Flags;

        [FieldOffset(11)]
        public byte Reserved;

        [FieldOffset(12)]
        public uint Reserved2;

        [FieldOffset(16)]
        public WinDivertDataNetwork Network;

        [FieldOffset(16)]
        public WinDivertDataFlow Flow;

        [FieldOffset(16)]
        public WinDivertDataSocket Socket;

        [FieldOffset(16)]
        public WinDivertDataReflect Reflect;

        [FieldOffset(16)]
        public fixed byte Reserved3[64];
    }
}
