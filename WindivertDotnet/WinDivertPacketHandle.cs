using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// WinDivertPacket的句柄 
    /// </summary>
    sealed class WinDivertPacketHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public int Capacity { get; }

        public WinDivertPacketHandle(int capacity)
            : base(true)
        {
            this.Capacity = capacity;
            base.SetHandle(Marshal.AllocHGlobal(capacity));
        }

        public unsafe Span<byte> GetSpan(int length)
        {
            return length > this.Capacity
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
