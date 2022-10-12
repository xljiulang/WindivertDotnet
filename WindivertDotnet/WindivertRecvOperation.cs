using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    sealed class WindivertRecvOperation : WindivertOperation
    {
        private readonly WinDivertHandle handle;
        private readonly WinDivertPacket packet;
        private readonly GCHandle addrLenHandle;

        public unsafe WindivertRecvOperation(
            WinDivertHandle handle,
            WinDivertPacket packet,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.handle = handle;
            this.packet = packet;
            this.addrLenHandle = GCHandle.Alloc(sizeof(WinDivertAddress), GCHandleType.Pinned);
        }

        protected override unsafe bool IOControl(ref int length, ref WinDivertAddress addr, NativeOverlapped* nativeOverlapped)
        {
            var pAddrLen = (int*)this.addrLenHandle.AddrOfPinnedObject().ToPointer();
            return WinDivertNative.WinDivertRecvEx(this.handle, this.packet, this.packet.Capacity, ref length, 0, ref addr, pAddrLen, nativeOverlapped);
        }

        protected override void SetResult(int length)
        {
            this.packet.Length = length;
            this.addrLenHandle.Free();
            base.SetResult(length);
        }

        protected override void SetException(int errorCode)
        {
            this.packet.Length = 0;
            this.addrLenHandle.Free();
            base.SetException(errorCode);
        }
    }
}
