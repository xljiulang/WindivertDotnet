using System;
using System.Diagnostics;
using System.Net;
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte localAddr[sizeof(int) * 4];

        /// <summary>
        /// 本机地址
        /// </summary>
        public unsafe IPAddress LocalAddr
        {
            get
            {
                fixed (void* ptr = this.localAddr)
                {
                    return this.GetIPAddress(ptr);
                }
            }
            set
            {
                fixed (void* ptr = this.localAddr)
                {
                    this.SetIPAddress(ptr, value);
                }
            }
        }

        /// <summary>
        /// 远程地址
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte remoteAddr[sizeof(int) * 4];

        public unsafe IPAddress RemoteAddr
        {
            get
            {
                fixed (void* ptr = this.remoteAddr)
                {
                    return this.GetIPAddress(ptr);
                }
            }
            set
            {
                fixed (void* ptr = this.remoteAddr)
                {
                    this.SetIPAddress(ptr, value);
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

        private unsafe IPAddress GetIPAddress(void* ptr)
        {
            var span = new Span<byte>(ptr, sizeof(int) * 4);
            span.Reverse();
            return new(span);
        }

        private unsafe void SetIPAddress(void* ptr, IPAddress value)
        {
            var span = value.GetAddressBytes().AsSpan();
            span.Reverse();
            span.CopyTo(new Span<byte>(ptr, sizeof(int) * 4));
        }
    }
}
