using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// Socket信息
    /// </summary>
    public unsafe struct WinDivertDataSocket
    {
        private const int IPV6_SIZE = sizeof(int) * 4;

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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte localAddr[IPV6_SIZE];

        /// <summary>
        /// 本机地址
        /// </summary>
        public IPAddress LocalAddr
        {
            get
            {
                fixed (void* ptr = this.localAddr)
                {
                    return GetIPAddress(ptr);
                }
            }
            set
            {
                fixed (void* ptr = this.localAddr)
                {
                    SetIPAddress(ptr, value);
                }
            }
        }

        /// <summary>
        /// 远程地址
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte remoteAddr[IPV6_SIZE];

        /// <summary>
        /// 远程地址
        /// </summary>
        public IPAddress RemoteAddr
        {
            get
            {
                fixed (void* ptr = this.remoteAddr)
                {
                    return GetIPAddress(ptr);
                }
            }
            set
            {
                fixed (void* ptr = this.remoteAddr)
                {
                    SetIPAddress(ptr, value);
                }
            }
        }

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

        private static IPAddress GetIPAddress(void* ptr)
        {
            var span = new Span<byte>(ptr, IPV6_SIZE);
            span.Reverse();
            return new(span);
        }

        private static void SetIPAddress(void* ptr, IPAddress value)
        {
            var span = value.GetAddressBytes().AsSpan();
            span.Reverse();
            span.CopyTo(new Span<byte>(ptr, IPV6_SIZE));
        }
    }
}
