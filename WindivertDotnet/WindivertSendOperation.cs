using System.Threading;

namespace WindivertDotnet
{
    sealed class WindivertSendOperation : WindivertOperation
    {
        private readonly WinDivertHandle handle;
        private readonly WinDivertPacket packet;

        public unsafe WindivertSendOperation(
            WinDivertHandle handle,
            WinDivertPacket packet,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.handle = handle;
            this.packet = packet;
        }

        protected override unsafe bool IOControl(int* pLength, ref WinDivertAddress addr, NativeOverlapped* nativeOverlapped)
        {
            var addrLength = sizeof(WinDivertAddress);
            return WinDivertNative.WinDivertSendEx(this.handle, this.packet, this.packet.Length, pLength, 0, ref addr, addrLength, nativeOverlapped);
        }
    }
}
