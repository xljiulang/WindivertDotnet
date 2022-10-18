using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// SockAddress结构体
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct SockAddress
    {
        private const int V4_SIZE = 4;
        private const int V6_SIZE = 16;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(0)]
        private ushort addressFamily;

        [FieldOffset(2)]
        public ushort Port;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(4)]
        private unsafe fixed byte addressV4[V4_SIZE];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(8)]
        private unsafe fixed byte addressV6[V6_SIZE];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(24)]
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
                    fixed (byte* ptr = this.addressV6)
                    {
                        return new IPAddress(new Span<byte>(ptr, V6_SIZE), this.scopeId);
                    }
                }
                throw new NotSupportedException($"不支持AddressFamily: {family}");
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
                    fixed (byte* ptr = this.addressV6)
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
