using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace WindivertDotnet
{
    [SupportedOSPlatform("windows")]
    sealed unsafe class WinDivertRecvOperation : WinDivertOperation
    {
        private readonly WinDivert divert;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public WinDivertRecvOperation(
            WinDivert divert,
            WinDivertPacket packet,
            WinDivertAddress addr) : base(divert)
        {
            this.divert = divert;
            this.packet = packet;
            this.addr = addr;
        }

        protected override bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped)
        {
            return WinDivertNative.WinDivertRecvEx(
                this.divert,
                this.packet,
                this.packet.Capacity,
                pLength,
                0UL,
                this.addr,
                null,
                nativeOverlapped);
        }

        protected override void SetResult(int length)
        {
            this.packet.Length = length;
            base.SetResult(length);
        }

        protected override void SetException(int errorCode)
        {
            this.packet.Length = 0;
            base.SetException(errorCode);
        }
    }
}
