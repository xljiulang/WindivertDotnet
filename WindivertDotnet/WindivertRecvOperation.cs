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
            ThreadPoolBoundHandle boundHandle,
            IOCompletionCallback completionCallback) : base(boundHandle, completionCallback)
        {
            this.handle = handle;
            this.packet = packet;
        }

        public unsafe override void IOControl(ref WinDivertAddress addr)
        {
            var length = 0;
            var addrLength = sizeof(WinDivertAddress);
            var flag = WinDivertNative.WinDivertRecvEx(this.handle, this.packet, this.packet.Capacity, ref length, 0, ref addr, &addrLength, this.NativeOverlapped);

            if (flag == true)
            {
                this.SetResult(length);
            }
        }

        public override void SetResult(int length)
        {
            this.packet.Length = length;
            base.SetResult(length);
        }

        public override void SetException(int errorCode)
        {
            this.packet.Length = 0;
            base.SetException(errorCode);
        }
    }
}
