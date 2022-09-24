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
            WINDIVERT_LAYER layer,
            short priority,
            WINDIVERT_FLAG flags);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertRecv(
            WinDivertHandle handle,
            IntPtr pPacket,
            int packetLen,
            ref int pRecvLen,
            ref WindivertAddress pAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertRecvEx([NativeTypeName("HANDLE")] WinDivertHandle handle, void* pPacket, uint packetLen, uint* pRecvLen, [NativeTypeName("UINT64")] ulong flags, WindivertAddress* pAddr, uint* pAddrLen, [NativeTypeName("LPOVERLAPPED")] NativeOverlapped* lpOverlapped);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertSend([NativeTypeName("HANDLE")] WinDivertHandle handle, [NativeTypeName("const void *")] void* pPacket, uint packetLen, uint* pSendLen, [NativeTypeName("const WINDIVERT_ADDRESS *")] WindivertAddress* pAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertSendEx([NativeTypeName("HANDLE")] WinDivertHandle handle, [NativeTypeName("const void *")] void* pPacket, uint packetLen, uint* pSendLen, [NativeTypeName("UINT64")] ulong flags, [NativeTypeName("const WINDIVERT_ADDRESS *")] WindivertAddress* pAddr, uint addrLen, [NativeTypeName("LPOVERLAPPED")] NativeOverlapped* lpOverlapped);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertShutdown([NativeTypeName("HANDLE")] WinDivertHandle handle, WINDIVERT_SHUTDOWN how);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern bool WinDivertClose([NativeTypeName("HANDLE")] IntPtr handle);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertSetParam([NativeTypeName("HANDLE")] WinDivertHandle handle, WINDIVERT_PARAM param1, [NativeTypeName("UINT64")] ulong value);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertGetParam([NativeTypeName("HANDLE")] WinDivertHandle handle, WINDIVERT_PARAM param1, [NativeTypeName("UINT64 *")] ulong* pValue);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT64")]
        public static extern ulong WinDivertHelperHashPacket([NativeTypeName("const void *")] void* pPacket, uint packetLen, [NativeTypeName("UINT64")] ulong seed = 0);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperParsePacket(
            IntPtr pPacket,
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
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperParseIPv4Address([NativeTypeName("const char *")] sbyte* addrStr, [NativeTypeName("UINT32 *")] uint* pAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperParseIPv6Address([NativeTypeName("const char *")] sbyte* addrStr, [NativeTypeName("UINT32 *")] uint* pAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperFormatIPv4Address([NativeTypeName("UINT32")] uint addr, [NativeTypeName("char *")] sbyte* buffer, uint bufLen);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperFormatIPv6Address([NativeTypeName("const UINT32 *")] uint* pAddr, [NativeTypeName("char *")] sbyte* buffer, uint bufLen);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperCalcChecksums(void* pPacket, uint packetLen, WindivertAddress* pAddr, [NativeTypeName("UINT64")] ulong flags);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperDecrementTTL(void* pPacket, uint packetLen);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperCompileFilter([NativeTypeName("const char *")] sbyte* filter, WINDIVERT_LAYER layer, [NativeTypeName("char *")] sbyte* @object, uint objLen, [NativeTypeName("const char **")] sbyte** errorStr, uint* errorPos);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperEvalFilter([NativeTypeName("const char *")] sbyte* filter, [NativeTypeName("const void *")] void* pPacket, uint packetLen, [NativeTypeName("const WINDIVERT_ADDRESS *")] WindivertAddress* pAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("BOOL")]
        public static extern int WinDivertHelperFormatFilter([NativeTypeName("const char *")] sbyte* filter, WINDIVERT_LAYER layer, [NativeTypeName("char *")] sbyte* buffer, uint bufLen);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT16")]
        public static extern ushort WinDivertHelperNtohs([NativeTypeName("UINT16")] ushort x);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT16")]
        public static extern ushort WinDivertHelperHtons([NativeTypeName("UINT16")] ushort x);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT32")]
        public static extern uint WinDivertHelperNtohl([NativeTypeName("UINT32")] uint x);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT32")]
        public static extern uint WinDivertHelperHtonl([NativeTypeName("UINT32")] uint x);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT64")]
        public static extern ulong WinDivertHelperNtohll([NativeTypeName("UINT64")] ulong x);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: NativeTypeName("UINT64")]
        public static extern ulong WinDivertHelperHtonll([NativeTypeName("UINT64")] ulong x);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void WinDivertHelperNtohIPv6Address([NativeTypeName("const UINT *")] uint* inAddr, uint* outAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void WinDivertHelperHtonIPv6Address([NativeTypeName("const UINT *")] uint* inAddr, uint* outAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void WinDivertHelperNtohIpv6Address([NativeTypeName("const UINT *")] uint* inAddr, uint* outAddr);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void WinDivertHelperHtonIpv6Address([NativeTypeName("const UINT *")] uint* inAddr, uint* outAddr);
    }
}
