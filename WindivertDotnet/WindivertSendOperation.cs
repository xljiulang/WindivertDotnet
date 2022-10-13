using System.Threading;

namespace WindivertDotnet
{
    sealed class WindivertSendOperation : WindivertOperation
    {
        private readonly WinDivert divert;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public unsafe WindivertSendOperation(
            WinDivert divert,
            WinDivertPacket packet,
            WinDivertAddress addr,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.divert = divert;
            this.packet = packet;
            this.addr = addr;
        }

        protected override unsafe bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped)
        {
            var addrLength = WinDivertAddress.Size;
            return WinDivertNative.WinDivertSendEx(this.divert, this.packet, this.packet.Length, pLength, 0, this.addr, addrLength, nativeOverlapped);
        }
    }
}
