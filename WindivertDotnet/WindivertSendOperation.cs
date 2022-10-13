using System.Threading;

namespace WindivertDotnet
{
    sealed class WindivertSendOperation : WindivertOperation
    {
        private readonly WinDivertHandle handle;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public unsafe WindivertSendOperation(
            WinDivertHandle handle,
            WinDivertPacket packet,
            WinDivertAddress addr,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.handle = handle;
            this.packet = packet;
            this.addr = addr;
        }

        protected override unsafe bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped)
        {
            var addrLength = WinDivertAddress.Size;
            return WinDivertNative.WinDivertSendEx(this.handle, this.packet, this.packet.Length, pLength, 0, this.addr, addrLength, nativeOverlapped);
        }
    }
}
