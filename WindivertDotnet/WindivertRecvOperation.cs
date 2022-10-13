using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    sealed unsafe class WinDivertRecvOperation : WinDivertOperation
    {
        private readonly WinDivert divert;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;
        private readonly IntPtr addrLenHandle = Marshal.AllocHGlobal(sizeof(int));

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
            var pAddrLen = (int*)addrLenHandle.ToPointer();
            *pAddrLen = WinDivertAddress.Size;

            return WinDivertNative.WinDivertRecvEx(
                this.divert,
                this.packet,
                this.packet.Capacity,
                pLength,
                0UL,
                this.addr,
                pAddrLen,
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

        public override void Dispose()
        {
            Marshal.FreeHGlobal(this.addrLenHandle);
            base.Dispose();
        }
    }
}
