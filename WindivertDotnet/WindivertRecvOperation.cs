using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    sealed unsafe class WindivertRecvOperation : WindivertOperation
    {
        private readonly WinDivert divert;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;
        private readonly int* pAddrLength = (int*)Marshal.AllocHGlobal(sizeof(int));

        public WindivertRecvOperation(
            WinDivert divert,
            WinDivertPacket packet,
            WinDivertAddress addr,
            ThreadPoolBoundHandle boundHandle) : base(boundHandle)
        {
            this.divert = divert;
            this.packet = packet;
            this.addr = addr;
        }

        protected override bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped)
        {
            *this.pAddrLength = WinDivertAddress.Size;
            return WinDivertNative.WinDivertRecvEx(this.divert, this.packet, this.packet.Capacity, this.pAddrLength, 0, this.addr, pAddrLength, nativeOverlapped);
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

        public override void Dispose()
        {
            Marshal.FreeHGlobal(new IntPtr(this.pAddrLength));
            base.Dispose();
        }
    }
}
