using System.Runtime.Versioning;

namespace System.Runtime.InteropServices
{
    [SupportedOSPlatform("windows")]
    unsafe static class IPHelpApiNative
    {
        private const string library = "iphlpapi.dll";

        [DllImport(library)]
        public extern static int GetBestInterfaceEx(
            byte* pDstSockAddr,
            out int index);

        [DllImport(library)]
        public static extern int GetBestRoute2(
            IntPtr interfaceLuid,
            int interfaceIndex,
            byte* sourceAddress,
            byte* destinationAddress,
            uint addressSortOptions,
            byte* bestRoute,
            byte* bestSourceAddress);
    }
}
