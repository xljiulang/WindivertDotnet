using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    sealed class WindivertRecvOperation : WindivertOperation
    {
        private readonly WinDivertHandle handle;
        private readonly WinDivertPacket packet;
        private readonly unsafe int* pAddrLen = (int*)Marshal.AllocHGlobal(sizeof(int));

        public unsafe WindivertRecvOperation(
            WinDivertHandle handle,
            WinDivertPacket packet,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.handle = handle;
            this.packet = packet;
        }

        protected override unsafe bool IOControl(int* pLength, ref WinDivertAddress addr, NativeOverlapped* nativeOverlapped)
        {
            *this.pAddrLen = sizeof(WinDivertAddress);
            return WinDivertNative.WinDivertRecvEx(this.handle, this.packet, this.packet.Capacity, this.pAddrLen, 0, ref addr, pAddrLen, nativeOverlapped);
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

        protected unsafe override void FreeNative()
        {
            Marshal.FreeHGlobal(new IntPtr(pAddrLen));
            base.FreeNative();
        }
    }
}
