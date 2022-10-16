using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace WindivertDotnet
{
    [SupportedOSPlatform("windows")]
    sealed class WinDivertRecvOperation : WinDivertOperation
    {
        private readonly WinDivertPacket packet;
        private readonly WinDivertAddress addr;

        public WinDivertRecvOperation(
            WinDivert divert,
            WinDivertPacket packet,
            WinDivertAddress addr) : base(divert)
        {
            this.packet = packet;
            this.addr = addr;
        }

        public override async ValueTask<int> IOControlAsync(CancellationToken cancellationToken)
        {
            var length = await base.IOControlAsync(cancellationToken);
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
