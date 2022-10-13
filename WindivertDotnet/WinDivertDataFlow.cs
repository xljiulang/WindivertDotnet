using System.Diagnostics;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// 网络流信息
    /// </summary>
    public unsafe struct WinDivertDataFlow
    {
        /// <summary>
        /// 流的终结点ID
        /// </summary>
        public long EndpointId;

        /// <summary>
        /// 流的父终结点ID
        /// </summary>
        public long ParentEndpointId;

        /// <summary>
        /// 与流关联的进程的ID
        /// </summary>
        public int ProcessId;

        /// <summary>
        /// 本机地址
        /// </summary>
        public fixed uint LocalAddr[4];

        /// <summary>
        /// 远程地址
        /// </summary>
        public fixed uint RemoteAddr[4];

        /// <summary>
        /// 本机端口
        /// </summary>
        public ushort LocalPort;

        /// <summary>
        /// 远程端口
        /// </summary>
        public ushort RemotePort;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte protocol;

        /// <summary>
        /// 协议类型
        /// </summary>
        public ProtocolType Protocol
        {
            get => (ProtocolType)protocol;
            set => protocol = (byte)value;
        }
    }
}
