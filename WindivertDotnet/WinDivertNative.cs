using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    static unsafe class WinDivertNative
    {
        private const string library = "WinDivert.dll";

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern WinDivertHandle WinDivertOpen(
            [MarshalAs(UnmanagedType.LPStr)] string filter,
            WinDivertLayer layer,
            short priority,
            WinDivertFlag flags); 


        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertRecvEx(
            WinDivertHandle handle,
            SafeHandle pPacket,
            int packetLen,
            ref int pRecvLen,
            ulong flags,
            ref WinDivertAddress pAddr,
            int* pAddrLen,
            NativeOverlapped* lpOverlapped); 
         


        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertSendEx(
            WinDivertHandle handle,
            SafeHandle pPacket,
            int packetLen,
            ref int pSendLen,
            ulong flags,
            ref WinDivertAddress pAddr,
            int addrLen,
            NativeOverlapped* lpOverlapped);


        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertShutdown(
            WinDivertHandle handle,
            WinDivertShutdown how);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertClose(
            IntPtr handle);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertSetParam(
            WinDivertHandle handle,
            WinDivertParam param1,
            long value);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertGetParam(
            WinDivertHandle handle,
            WinDivertParam param1,
            ref long pValue);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int WinDivertHelperHashPacket(
            SafeHandle pPacket,
            int packetLen,
            long seed = 0);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperParsePacket(
            SafeHandle pPacket,
            int packetLen,
            IPV4Header** ppIpHdr,
            IPV6Header** ppIpv6Hdr,
            byte* pProtocol,
            IcmpV4Header** ppIcmpHdr,
            IcmpV6Header** ppIcmpv6Hdr,
            TcpHeader** ppTcpHdr,
            UdpHeader** ppUdpHdr,
            byte** ppData,
            int* pDataLen,
            byte** ppNext,
            int* pNextLen);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperCalcChecksums(
          SafeHandle pPacket,
          int packetLen,
          ref WinDivertAddress pAddr,
          ChecksumsFlag flags);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperDecrementTTL(
            SafeHandle pPacket,
            int packetLen);
    }
}
