using System.Runtime.Versioning;
using System.Threading;

namespace System.Runtime.InteropServices
{
    [SupportedOSPlatform("windows")]
    static class Kernel32Native
    {
        private const string library = "kernel32.dll";

        [DllImport(library, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);


        [DllImport(library, SetLastError = true)]
        public unsafe static extern bool CancelIoEx(SafeHandle handle, NativeOverlapped* lpOverlapped);
    }
}
