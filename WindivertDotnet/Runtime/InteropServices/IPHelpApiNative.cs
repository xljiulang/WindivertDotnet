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
            ref SockAddress dstSockAddr,
            out int index);

        [DllImport(library)]
        public static extern int GetBestRoute2(
            IntPtr interfaceLuid,
            int interfaceIndex,
            SockAddress* pSrcSocketAddr,
            ref SockAddress dstSocketAddr,
            uint addressSortOptions,
            byte* pBestRoute, // 103字节
            ref SockAddress bestSrcSocketAddr);
    }
}
