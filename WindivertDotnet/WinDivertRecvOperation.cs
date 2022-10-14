using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace WindivertDotnet
{
    [SupportedOSPlatform("windows")]
    sealed class WinDivertRecvOperation : WinDivertOperation
    {
        private readonly WinDivert divert;
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public WinDivertRecvOperation(
            WinDivert divert,
            WinDivertPacket packet,
            WinDivertAddress addr) : base(divert)
        {
            this.divert = divert;
            this.packet = packet;
            this.addr = addr;
        }

        public override async ValueTask<int> IOControlAsync()
        {
            var length = await base.IOControlAsync();
            this.packet.Length = length;
            return length;
        }

        protected override unsafe bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped)
        {
            return WinDivertNative.WinDivertRecvEx(
                this.divert,
                this.packet,
                this.packet.Capacity,
                pLength,
                0UL,
                this.addr,
                null,
                nativeOverlapped);
        }
    }
}
