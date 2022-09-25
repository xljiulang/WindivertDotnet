using Microsoft.Win32.SafeHandles;

namespace WindivertDotnet
{
    /// <summary>
    /// WinDivert的句柄
    /// </summary>
    sealed class WinDivertHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private WinDivertHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return WinDivertNative.WinDivertClose(this.handle);
        }
    }
}
