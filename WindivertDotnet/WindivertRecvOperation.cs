using System.Threading;

namespace WindivertDotnet
{
    sealed class WindivertRecvOperation : WindivertOperation
    {
        private readonly WinDivertHandle handle;
        private readonly WinDivertPacket packet;

        public unsafe WindivertRecvOperation(
            WinDivertHandle handle,
            WinDivertPacket packet,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.handle = handle;
            this.packet = packet;
        }

        protected override unsafe bool IOControl(ref int length, ref WinDivertAddress addr, NativeOverlapped* nativeOverlapped)
        {
            var addrLength = sizeof(WinDivertAddress);
            return WinDivertNative.WinDivertRecvEx(this.handle, this.packet, this.packet.Capacity, ref length, 0, ref addr, &addrLength, nativeOverlapped);
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
