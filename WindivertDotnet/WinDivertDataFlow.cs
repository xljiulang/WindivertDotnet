using System.ComponentModel;
using System.Net.Sockets;

namespace WindivertDotnet
{
    public unsafe struct WinDivertDataFlow
    {
        public long EndpointId;

        public long ParentEndpointId;

        public int ProcessId;

        public fixed uint LocalAddr[4];

        public fixed uint RemoteAddr[4];

        public ushort LocalPort;

        public ushort RemotePort;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private byte protocol;
        public ProtocolType Protocol => (ProtocolType)protocol;
    }
}
