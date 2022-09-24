using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct WinDivertAddress
    {
        [FieldOffset(0)]
        public long Timestamp;

        [FieldOffset(8)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private uint _bitfield;

        public uint Layer
        {
            get => _bitfield & 0xFFu;
            set => _bitfield = (_bitfield & ~0xFFu) | (value & 0xFFu);
        }


        public uint Event
        {
            get => (_bitfield >> 8) & 0xFFu;
            set => _bitfield = (_bitfield & ~(0xFFu << 8)) | ((value & 0xFFu) << 8);
        }


        public bool Sniffed
        {
            get => ((_bitfield >> 16) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 16)) | (((value ? 1u : 0u) & 0x1u) << 16);
        }


        public bool Outbound
        {
            get => ((_bitfield >> 17) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 17)) | (((value ? 1u : 0u) & 0x1u) << 17);
        }


        public bool Loopback
        {
            get => ((_bitfield >> 18) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 18)) | (((value ? 1u : 0u) & 0x1u) << 18);
        }

        public bool Impostor
        {
            get => ((_bitfield >> 19) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 19)) | (((value ? 1u : 0u) & 0x1u) << 19);
        }

        public bool IPv6
        {
            get => ((_bitfield >> 20) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 20)) | (((value ? 1u : 0u) & 0x1u) << 20);
        }

        public bool IPChecksum
        {
            get => ((_bitfield >> 21) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 21)) | (((value ? 1u : 0u) & 0x1u) << 21);
        }

        public bool TCPChecksum
        {
            get => ((_bitfield >> 22) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 22)) | (((value ? 1u : 0u) & 0x1u) << 22);
        }

        public bool UDPChecksum
        {
            get => ((_bitfield >> 23) & 0x1u) == 1;
            set => _bitfield = (_bitfield & ~(0x1u << 23)) | (((value ? 1u : 0u) & 0x1u) << 23);
        }

        public uint Reserved1
        {
            get => (_bitfield >> 24) & 0xFFu;
            set => _bitfield = (_bitfield & ~(0xFFu << 24)) | ((value & 0xFFu) << 24);
        }

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
