using System;
using System.Buffers.Binary;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert路由
    /// </summary>
    [SupportedOSPlatform("windows")]
    public unsafe class WinDivertRouter
    {
        private const int SocketAddress_SIZE = 28;
        private const int MIB_IPFORWARD_ROW2_SIZE = 103;

        /// <summary>
        /// 获取目标地址
        /// </summary>
        public IPAddress DstAddress { get; }

        /// <summary>
        /// 获取源地址
        /// </summary>
        public IPAddress SrcAddress { get; }

        /// <summary>
        /// 获取网络接口索引
        /// </summary>
        public int InterfaceIndex { get; }

        /// <summary>
        /// 获取是否为出口方向
        /// </summary>
        public bool IsOutbound { get; }

        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param> 
        /// <exception cref="ArgumentException"></exception> 
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public WinDivertRouter(IPAddress dstAddr)
            : this(dstAddr, srcAddr: null, interfaceIndex: null)
        {
        }

        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param>
        /// <param name="srcAddr">源地址</param> 
        /// <exception cref="ArgumentException"></exception> 
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public WinDivertRouter(IPAddress dstAddr, IPAddress srcAddr)
            : this(dstAddr, srcAddr, interfaceIndex: null)
        {
        }


        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param>
        /// <param name="srcAddr">源地址</param> 
        /// <param name="interfaceIndex">网络接口索引</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public WinDivertRouter(IPAddress dstAddr, IPAddress srcAddr, int interfaceIndex)
            : this(dstAddr, srcAddr, (int?)interfaceIndex)
        {
        }

        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param>
        /// <param name="srcAddr">源地址</param>
        /// <param name="interfaceIndex">网络接口索引</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private WinDivertRouter(IPAddress dstAddr, IPAddress? srcAddr, int? interfaceIndex)
        {
            if (IsIPAddressAny(dstAddr))
            {
                throw new ArgumentException($"值不能为{dstAddr}", nameof(dstAddr));
            }

            if (srcAddr != null && srcAddr.AddressFamily != dstAddr.AddressFamily)
            {
                throw new ArgumentException($"{srcAddr}和{dstAddr}的AddressFamily不一致", nameof(srcAddr));
            }

            if (IsIPAddressAny(srcAddr)) // any -> null
            {
                srcAddr = null;
            }

            if (InterfaceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interfaceIndex));
            }

            byte* srcSockAddr = null;
            if (srcAddr != null)
            {
                var sockAddr = stackalloc byte[SocketAddress_SIZE];
                SetIPAddress(sockAddr, srcAddr);
                srcSockAddr = sockAddr;
            }

            var dstSockAddr = stackalloc byte[SocketAddress_SIZE];
            SetIPAddress(dstSockAddr, dstAddr);

            interfaceIndex ??= GetInterfaceIndex(dstSockAddr);

            var bestRoute = stackalloc byte[MIB_IPFORWARD_ROW2_SIZE];
            var bestSrcSockAddr = stackalloc byte[SocketAddress_SIZE];

            var errorCode = IPHelpApiNative.GetBestRoute2(
                IntPtr.Zero,
                interfaceIndex.Value,
                srcSockAddr,
                dstSockAddr,
                0,
                bestRoute,
                bestSrcSockAddr);

            if (errorCode != 0 && srcAddr == null)
            {
                throw new NotSupportedException($"无法在网络接口索引{interfaceIndex}获取SrcAddress");
            }

            this.SrcAddress = srcAddr ?? GetIPAddress(bestSrcSockAddr);
            this.DstAddress = dstAddr;
            this.InterfaceIndex = interfaceIndex.Value;
            this.IsOutbound = errorCode == 0;
        }

        /// <summary>
        /// 获取网络接口索引
        /// </summary>
        /// <param name="dstAddr">目标地址</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NetworkInformationException"></exception>
        public static int GetInterfaceIndex(IPAddress dstAddr)
        {
            if (IsIPAddressAny(dstAddr))
            {
                throw new ArgumentException($"值不能为{dstAddr}", nameof(dstAddr));
            }

            var dstSockAddr = stackalloc byte[SocketAddress_SIZE];
            SetIPAddress(dstSockAddr, dstAddr);
            return GetInterfaceIndex(dstSockAddr);
        }


        /// <summary>
        /// 是否为any的ip
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static bool IsIPAddressAny(IPAddress? address)
        {
            return address != null && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any));
        }

        /// <summary>
        /// 获取网络接口索引
        /// </summary>
        /// <param name="dstSockAddr"></param>
        /// <returns></returns>
        /// <exception cref="NetworkInformationException"></exception>
        private static int GetInterfaceIndex(byte* dstSockAddr)
        {
            var errorCode = IPHelpApiNative.GetBestInterfaceEx(dstSockAddr, out var ifIdx);
            return errorCode == 0 ? ifIdx : throw new NetworkInformationException(errorCode);
        }

        /// <summary>
        /// 填充IP到pSockAddr
        /// </summary>
        /// <param name="pSockAddr"></param>
        /// <param name="addr"></param>
        private static void SetIPAddress(byte* pSockAddr, IPAddress addr)
        {
            var span = new Span<byte>(pSockAddr, SocketAddress_SIZE);
            BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort)addr.AddressFamily);

            if (addr.AddressFamily == AddressFamily.InterNetworkV6)
            {
                addr.TryWriteBytes(span[8..], out _);
                BinaryPrimitives.WriteUInt32LittleEndian(span[24..], (uint)addr.ScopeId);
            }
            else
            {
                addr.TryWriteBytes(span[4..], out _);
            }
        }

        /// <summary>
        /// 从pSockAddr获取IP地址
        /// </summary>
        /// <param name="pSockAddr"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static IPAddress GetIPAddress(byte* pSockAddr)
        {
            var span = new Span<byte>(pSockAddr, SocketAddress_SIZE);
            var addressFamily = (AddressFamily)BinaryPrimitives.ReadInt16LittleEndian(span);
            if (addressFamily == AddressFamily.InterNetwork)
            {
                var address = span.Slice(4, 4);
                return new IPAddress(address);
            }
            else if (addressFamily == AddressFamily.InterNetworkV6)
            {
                var address = span.Slice(8, 16);
                var scope = BinaryPrimitives.ReadUInt32LittleEndian(span[24..]);
                return new IPAddress(address, scope);
            }
            throw new NotSupportedException($"不支持AddressFamily: {addressFamily}");
        }
    }
}
