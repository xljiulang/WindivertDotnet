using System;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    sealed class WinDivertHandle : SafeHandle
    {
        private static readonly IntPtr invalidHandleValue = new(unchecked((long)ulong.MaxValue));

        private WinDivertHandle()
            : base(invalidHandleValue, ownsHandle: true)
        {
        }

        public override bool IsInvalid => this.handle == invalidHandleValue;

        protected override bool ReleaseHandle()
        {
            return WinDivertNative.WinDivertClose(this.handle);
        }
    }
}
