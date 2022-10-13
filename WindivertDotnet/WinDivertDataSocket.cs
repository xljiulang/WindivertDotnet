using System.Diagnostics;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// Socket信息
    /// </summary>
    public unsafe struct WinDivertDataSocket
    {
        /// <summary>
        /// 套接字操作的终结点 ID
        /// </summary>
        public long EndpointId;

        /// <summary>
        /// 套接字操作的父终结点 ID
        /// </summary>
        public long ParentEndpointId;

        /// <summary>
        /// 与套接字操作关联的进程的 ID
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
