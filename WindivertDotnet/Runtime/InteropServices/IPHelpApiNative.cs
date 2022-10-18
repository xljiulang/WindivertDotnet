using System.Runtime.Versioning;
using WindivertDotnet;

namespace System.Runtime.InteropServices
{
    [SupportedOSPlatform("windows")]
    unsafe static class IPHelpApiNative
    {
        private const string library = "iphlpapi.dll";

        [DllImport(library)]
        public extern static int GetBestInterfaceEx(
            SockAddress* pDstSockAddr,
            out int index);

        [DllImport(library)]
        public static extern int GetBestRoute2(
            IntPtr interfaceLuid,
            int interfaceIndex,
            SockAddress* pSrcSocketAddr,
            SockAddress* pDstSocketAddr,
            uint addressSortOptions,
            byte* pBestRoute, // 103字节
            SockAddress* pBestSrcSocketAddr);
    }
}
