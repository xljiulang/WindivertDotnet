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
