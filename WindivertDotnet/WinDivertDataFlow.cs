using System.Diagnostics;
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte protocol;

        /// <summary>
        /// 获取或设置协议类型
        /// </summary>
        public ProtocolType Protocol
        {
            get => (ProtocolType)protocol;
            set => protocol = (byte)value;
        }
    }
}
