using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    static unsafe class WinDivertNative
    {
        private const string library = "WinDivert.dll";

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern IntPtr WinDivertOpen(
            [MarshalAs(UnmanagedType.LPStr)] string filter,
            WinDivertLayer layer,
            short priority,
            WinDivertFlag flags);


        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertRecvEx(
            WinDivert divert,
            WinDivertPacket packet,
            int packetLen,
            int* pRecvLen,
            ulong flags,
            WinDivertAddress addr,
            int* pAddrLen,
            NativeOverlapped* lpOverlapped);



        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertSendEx(
            WinDivert divert,
            WinDivertPacket packet,
            int packetLen,
            int* pSendLen,
            ulong flags,
            WinDivertAddress addr,
            int addrLen,
            NativeOverlapped* lpOverlapped);


        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertShutdown(
            WinDivert divert,
            WinDivertShutdown how);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertClose(
            WinDivert divert);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertSetParam(
            WinDivert divert,
            WinDivertParam param1,
            long value);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertGetParam(
            WinDivert divert,
            WinDivertParam param1,
            ref long pValue);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int WinDivertHelperHashPacket(
            WinDivertPacket packet,
            int packetLen,
            long seed = 0);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperParsePacket(
            WinDivertPacket packet,
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
          WinDivertPacket packet,
          int packetLen,
          WinDivertAddress addr,
          ChecksumsFlag flags);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperDecrementTTL(
            WinDivertPacket packet,
            int packetLen);
    }
}
