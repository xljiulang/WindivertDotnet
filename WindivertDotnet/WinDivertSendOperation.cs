using System.Threading;

namespace WindivertDotnet
{
    sealed unsafe class WinDivertSendOperation : WinDivertOperation
    {
        private readonly WinDivert divert;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public WinDivertSendOperation(
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
