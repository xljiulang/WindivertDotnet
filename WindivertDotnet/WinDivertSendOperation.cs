using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace WindivertDotnet
{
    [SupportedOSPlatform("windows")]
    sealed class WinDivertSendOperation : WinDivertOperation
    {
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public WinDivertSendOperation(
            WinDivert divert,
            WinDivertPacket packet,
            WinDivertAddress addr) : base(divert)
        {
            this.packet = packet;
            this.addr = addr;
        }

        protected override unsafe bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped)
        {
            return WinDivertNative.WinDivertSendEx(
                this.divert,
                this.packet,
                this.packet.Length,
                pLength,
                0UL,
                this.addr,
                WinDivertAddress.Size,
                nativeOverlapped);
        }
    }
}
