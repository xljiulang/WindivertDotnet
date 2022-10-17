using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices
{
    [SupportedOSPlatform("windows")]
    unsafe static class IPHelpApiNative
    {
        private const string library = "iphlpapi.dll";
        private const int SocketAddress_SIZE = 28;
        private const int MIB_IPFORWARD_ROW2_SIZE = 103;

        [DllImport(library)]
        private extern static int GetBestInterfaceEx(
            byte* pDstSockAddr,
            out int index);

        [DllImport(library)]
        private static extern int GetBestRoute2(
            IntPtr interfaceLuid,
            int interfaceIndex,
            byte* sourceAddress,
            byte* destinationAddress,
            uint addressSortOptions,
            byte* bestRoute,
            byte* bestSourceAddress);

        /// <summary>
        /// 获取与目标地址通讯的网络接口索引
        /// </summary>
        /// <param name="dstAddress">目的地址</param>
        /// <returns></returns>
        /// <exception cref="NetworkInformationException"></exception>
        public static int GetInterfaceIndex(IPAddress dstAddress)
        {
            var sockAddr = stackalloc byte[SocketAddress_SIZE];
            SetIPAddress(sockAddr, dstAddress);
            var errorCode = GetBestInterfaceEx(sockAddr, out int index);
            return errorCode == 0 ? index : throw new NetworkInformationException(errorCode);
        }

        /// <summary>
        /// 获取指定网络接口的通讯数据是否为出口方向
        /// </summary>
        /// <param name="interfaceIndex">网络接口索引</param>
        /// <param name="srcAddr">源地址</param>
        /// <param name="dstAddr">目的地址</param>
        /// <returns></returns>
        public static bool IsOutboundDirection(int interfaceIndex, IPAddress srcAddr, IPAddress dstAddr)
        {
            var srcSockAddr = stackalloc byte[SocketAddress_SIZE];
            SetIPAddress(srcSockAddr, srcAddr);

            var dstSockAddr = stackalloc byte[SocketAddress_SIZE];
            SetIPAddress(dstSockAddr, dstAddr);

            var bestRoute = stackalloc byte[MIB_IPFORWARD_ROW2_SIZE];
            var bestSrcSockAddr = stackalloc byte[SocketAddress_SIZE];

            var errorCode = GetBestRoute2(IntPtr.Zero, interfaceIndex, srcSockAddr, dstSockAddr, 0, bestRoute, bestSrcSockAddr);
            return errorCode == 0;
        }

        /// <summary>
        /// 尝试获取本机地址
        /// 用于与remoteAddr进行出口通讯
        /// </summary>
        /// <param name="remoteAddr">远程地址</param>
        /// <param name="localAddr">本机地址</param>
        /// <param name="interfaceIndex">网络接口索引</param>
        /// <returns></returns>
        public static bool TryGetLocalAddress(
            IPAddress remoteAddr,
            [MaybeNullWhen(false)] out IPAddress localAddr,
            out int interfaceIndex)
        {
            var remoteSockAddr = stackalloc byte[SocketAddress_SIZE];
            SetIPAddress(remoteSockAddr, remoteAddr);

            var errorCode = GetBestInterfaceEx(remoteSockAddr, out interfaceIndex);
            if (errorCode == 0)
            {
                var bestRoute = stackalloc byte[MIB_IPFORWARD_ROW2_SIZE];
                var bestSrcSockAddr = stackalloc byte[SocketAddress_SIZE];

                errorCode = GetBestRoute2(IntPtr.Zero, interfaceIndex, null, remoteSockAddr, 0, bestRoute, bestSrcSockAddr);
                if (errorCode == 0)
                {
                    localAddr = GetIPAddress(bestSrcSockAddr);
                    return true;
                }
            }

            localAddr = default;
            return false;
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
