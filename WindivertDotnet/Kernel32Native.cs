using System;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    static class Kernel32Native
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
    }
}
