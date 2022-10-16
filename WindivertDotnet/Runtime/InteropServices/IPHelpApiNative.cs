using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices
{
    [SupportedOSPlatform("windows")]
    unsafe static class IPHelpApiNative
    {
        private const string library = "iphlpapi.dll";

        [DllImport(library)]
        private extern static int GetBestInterfaceEx(byte* pDstSockAddr, out int index);

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
            var sockAddr = stackalloc byte[28];
            FillSocketAddress(sockAddr, dstAddress);
            var errorCode = GetBestInterfaceEx(sockAddr, out int index);
            return errorCode == 0 ? index : throw new NetworkInformationException(errorCode);
        }

        /// <summary>
        /// 获取指定网络接口的通讯数据是否为出口方向
        /// </summary>
        /// <param name="interfaceIndex"></param>
        /// <param name="srcAddr"></param>
        /// <param name="dstAddr"></param>
        /// <returns></returns>
        public static bool IsOutboundDirection(int interfaceIndex, IPAddress srcAddr, IPAddress dstAddr)
        {
            var srcSockAddr = stackalloc byte[28];
            FillSocketAddress(srcSockAddr, srcAddr);

            var dstSockAddr = stackalloc byte[28];
            FillSocketAddress(dstSockAddr, dstAddr);

            var bestRoute = stackalloc byte[103];// sizeof(MIB_IPFORWARD_ROW2)
            var bestSrcSockAddr = stackalloc byte[28];

            var errorCode = GetBestRoute2(default, interfaceIndex, srcSockAddr, dstSockAddr, 0, bestRoute, bestSrcSockAddr);
            return errorCode == 0;
        }


        /// <summary>
        /// 填充pSockAddr
        /// </summary>
        /// <param name="pSockAddr"></param>
        /// <param name="addr"></param>
        private static void FillSocketAddress(byte* pSockAddr, IPAddress addr)
        {
            var bytes = new IPEndPoint(addr, 0).Serialize();
            for (int i = 0; i < bytes.Size; i++)
            {
                pSockAddr[i] = bytes[i];
            }
        }
    }
}
