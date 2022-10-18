using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// SockAddress结构体
    /// </summary> 
    unsafe struct SockAddress
    {
        private const int V4_SIZE = 4;
        private const int V6_SIZE = 16;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort addressFamily;

        public ushort Port;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte addressV4[V4_SIZE];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private fixed byte addressV6[V6_SIZE];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private uint scopeId;

        /// <summary>
        /// 获取或设置IP
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public IPAddress IPAddress
        {
            get
            {
                var family = (AddressFamily)this.addressFamily;
                if (family == AddressFamily.InterNetwork)
                {
                    fixed (void* ptr = this.addressV4)
                    {
                        return new IPAddress(new Span<byte>(ptr, V4_SIZE));
                    }
                }
                else if (family == AddressFamily.InterNetworkV6)
                {
                    fixed (void* ptr = this.addressV6)
                    {
                        return new IPAddress(new Span<byte>(ptr, V6_SIZE), this.scopeId);
                    }
                }
                throw new NotSupportedException($"不支持的AddressFamily: {family}");
            }
            set
            {
                this.addressFamily = (ushort)value.AddressFamily;
                if (value.AddressFamily == AddressFamily.InterNetwork)
                {
                    fixed (void* ptr = this.addressV4)
                    {
                        var span = new Span<byte>(ptr, V4_SIZE);
                        value.TryWriteBytes(span, out _);
                    }
                }
                else if (value.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    fixed (void* ptr = this.addressV6)
                    {
                        var span = new Span<byte>(ptr, V6_SIZE);
                        value.TryWriteBytes(span, out _);
                        this.scopeId = (uint)value.ScopeId;
                    }
                }
                else
                {
                    throw new NotSupportedException($"不支持AddressFamily: {value.AddressFamily}");
                }
            }
        }
    }
}
