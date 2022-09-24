using System;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    public unsafe struct WindivertAddress
    {
        [NativeTypeName("INT64")]
        public long Timestamp;

        public uint _bitfield;

        [NativeTypeName("UINT32 : 8")]
        public uint Layer
        {
            get
            {
                return _bitfield & 0xFFu;
            }

            set
            {
                _bitfield = (_bitfield & ~0xFFu) | (value & 0xFFu);
            }
        }

        [NativeTypeName("UINT32 : 8")]
        public uint Event
        {
            get
            {
                return (_bitfield >> 8) & 0xFFu;
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 8)) | ((value & 0xFFu) << 8);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint Sniffed
        {
            get
            {
                return (_bitfield >> 16) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 16)) | ((value & 0x1u) << 16);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint Outbound
        {
            get
            {
                return (_bitfield >> 17) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 17)) | ((value & 0x1u) << 17);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint Loopback
        {
            get
            {
                return (_bitfield >> 18) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 18)) | ((value & 0x1u) << 18);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint Impostor
        {
            get
            {
                return (_bitfield >> 19) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 19)) | ((value & 0x1u) << 19);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint IPv6
        {
            get
            {
                return (_bitfield >> 20) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 20)) | ((value & 0x1u) << 20);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint IPChecksum
        {
            get
            {
                return (_bitfield >> 21) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 21)) | ((value & 0x1u) << 21);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint TCPChecksum
        {
            get
            {
                return (_bitfield >> 22) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 22)) | ((value & 0x1u) << 22);
            }
        }

        [NativeTypeName("UINT32 : 1")]
        public uint UDPChecksum
        {
            get
            {
                return (_bitfield >> 23) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 23)) | ((value & 0x1u) << 23);
            }
        }

        [NativeTypeName("UINT32 : 8")]
        public uint Reserved1
        {
            get
            {
                return (_bitfield >> 24) & 0xFFu;
            }

            set
            {
                _bitfield = (_bitfield & ~(0xFFu << 24)) | ((value & 0xFFu) << 24);
            }
        }

        [NativeTypeName("UINT32")]
        public uint Reserved2;

        [NativeTypeName("WINDIVERT_ADDRESS::(anonymous union at windivert.h:157:5)")]
        public _Anonymous_e__Union Anonymous;

        public ref WINDIVERT_DATA_NETWORK Network
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Network, 1));
            }
        }

        public ref WINDIVERT_DATA_FLOW Flow
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Flow, 1));
            }
        }

        public ref WINDIVERT_DATA_SOCKET Socket
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Socket, 1));
            }
        }

        public ref WINDIVERT_DATA_REFLECT Reflect
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Reflect, 1));
            }
        }

        public Span<byte> Reserved3
        {
            get
            {
                return MemoryMarshal.CreateSpan(ref Anonymous.Reserved3[0], 64);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public WINDIVERT_DATA_NETWORK Network;

            [FieldOffset(0)]
            public WINDIVERT_DATA_FLOW Flow;

            [FieldOffset(0)]
            public WINDIVERT_DATA_SOCKET Socket;

            [FieldOffset(0)]
            public WINDIVERT_DATA_REFLECT Reflect;

            [FieldOffset(0)]
            [NativeTypeName("UINT8[64]")]
            public fixed byte Reserved3[64];
        }
    }
}
