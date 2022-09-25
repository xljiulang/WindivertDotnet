using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    sealed class WinDivertPacketHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly int capacity;

        public WinDivertPacketHandle(int capacity)
            : base(true)
        {
            this.capacity = capacity;
            base.SetHandle(Marshal.AllocHGlobal(capacity));
        }

        public unsafe Span<byte> GetSpan(int length)
        {
            return length > this.capacity
                ? throw new ArgumentOutOfRangeException(nameof(length))
                : new Span<byte>(this.handle.ToPointer(), length);
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(this.handle);
            return true;
        }
    }
}
