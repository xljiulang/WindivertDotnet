using System;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    sealed class WinDivertHandle : SafeHandle
    {
        private WinDivertHandle()
            : base(invalidHandleValue: IntPtr.MaxValue, ownsHandle: true)
        {
        }

        public override bool IsInvalid => this.handle == IntPtr.MaxValue;

        protected override bool ReleaseHandle()
        {
            return WinDivertNative.WinDivertClose(this.handle);
        }
    }
}
