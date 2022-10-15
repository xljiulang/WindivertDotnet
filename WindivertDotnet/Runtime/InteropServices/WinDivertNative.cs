using System.IO;
using System.Threading;
using WindivertDotnet;

namespace System.Runtime.InteropServices
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
        public static extern bool WinDivertClose(
            IntPtr handle);


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
        public static extern bool WinDivertHelperFormatFilter(
             [MarshalAs(UnmanagedType.LPStr)] string filter,
             WinDivertLayer layer,
             void* buffer,
             int bufLen);

        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern bool WinDivertHelperCompileFilter(
            [MarshalAs(UnmanagedType.LPStr)] string filter,
            WinDivertLayer layer,
            void* obj,
            int objLen,
            out IntPtr errorStr,
            out int errorPos);


        [DllImport(library, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int WinDivertHelperHashPacket(
            WinDivertPacket packet,
            int packetLen,
            long seed);

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


        static WinDivertNative()
        {
            if (Environment.Is64BitProcess)
            {
                LoadResource("WindivertDotnet.v222.x64.WinDivert64.sys");
                LoadResource("WindivertDotnet.v222.x64.WinDivert.dll", true);
            }
            else
            {
                LoadResource("WindivertDotnet.v222.x86.WinDivert32.sys");
                LoadResource("WindivertDotnet.v222.x86.WinDivert64.sys");
                LoadResource("WindivertDotnet.v222.x86.WinDivert.dll", true);
            }
        }

        /// <summary>
        /// º”‘ÿ◊ ‘¥
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loadLibray"></param>
        private static void LoadResource(string name, bool loadLibray = false)
        {
            var fileName = Path.GetFileNameWithoutExtension(name).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(name);
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var destFilePath = Path.Combine(appDataPath, fileName);

            if (File.Exists(destFilePath) == false)
            {
                var dir = Path.GetDirectoryName(destFilePath);
                if (string.IsNullOrEmpty(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                using var rsStream = typeof(WinDivert).Assembly.GetManifestResourceStream(name)!;
                using var fileStream = File.OpenWrite(destFilePath);
                rsStream.CopyTo(fileStream);
            }

            if (loadLibray == true)
            {
                Kernel32Native.LoadLibrary(destFilePath);
            }
        }
    }
}
