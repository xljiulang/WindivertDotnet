using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert路由
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class WinDivertRouter
    {
        private const int MIB_IPFORWARD_ROW2_SIZE = 103;

        /// <summary>
        /// 获取目标地址
        /// </summary>
        public IPAddress DstAddress { get; }

        /// <summary>
        /// 获取源地址
        /// </summary>
        public IPAddress SrcAddress { get; }

        /// <summary>
        /// 获取网络接口索引
        /// </summary>
        public int InterfaceIndex { get; }

        /// <summary>
        /// 获取是否为出口方向
        /// </summary>
        public bool IsOutbound { get; }

        /// <summary>
        /// 获取是否为回环
        /// </summary>
        public bool IsLoopback { get; }

        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param> 
        /// <exception cref="ArgumentException"></exception> 
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public WinDivertRouter(IPAddress dstAddr)
            : this(dstAddr, srcAddr: null, interfaceIndex: null)
        {
        }

        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param>
        /// <param name="srcAddr">源地址</param> 
        /// <exception cref="ArgumentException"></exception> 
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public WinDivertRouter(IPAddress dstAddr, IPAddress srcAddr)
            : this(dstAddr, srcAddr, interfaceIndex: null)
        {
        }


        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param>
        /// <param name="srcAddr">源地址</param> 
        /// <param name="interfaceIndex">网络接口索引</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public WinDivertRouter(IPAddress dstAddr, IPAddress srcAddr, int interfaceIndex)
            : this(dstAddr, srcAddr, (int?)interfaceIndex)
        {
        }

        /// <summary>
        /// WinDivert路由
        /// </summary> 
        /// <param name="dstAddr">目标地址</param>
        /// <param name="srcAddr">源地址</param>
        /// <param name="interfaceIndex">网络接口索引</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NetworkInformationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private unsafe WinDivertRouter(IPAddress dstAddr, IPAddress? srcAddr, int? interfaceIndex)
        {
            if (IsIPAddressAny(dstAddr))
            {
                throw new ArgumentException($"值不能为{dstAddr}", nameof(dstAddr));
            }

            if (srcAddr != null && srcAddr.AddressFamily != dstAddr.AddressFamily)
            {
                throw new ArgumentException($"{srcAddr}和{dstAddr}的AddressFamily不一致", nameof(srcAddr));
            }

            if (IsIPAddressAny(srcAddr)) // any -> null
            {
                srcAddr = null;
            }

            if (InterfaceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interfaceIndex));
            }

            SockAddress* pSrcSockAddr = null;
            if (srcAddr != null)
            {
                var sockAddr = new SockAddress { IPAddress = srcAddr };
                pSrcSockAddr = &sockAddr;
            }

            var dstSockAddr = new SockAddress { IPAddress = dstAddr };
            interfaceIndex ??= GetInterfaceIndex(ref dstSockAddr);

            var pBestRoute = stackalloc byte[MIB_IPFORWARD_ROW2_SIZE];
            var bestSrcSockAddr = new SockAddress();

            var errorCode = IPHelpApiNative.GetBestRoute2(
                IntPtr.Zero,
                interfaceIndex.Value,
                pSrcSockAddr,
                ref dstSockAddr,
                0U,
                pBestRoute,
                ref bestSrcSockAddr);

            if (errorCode != 0 && srcAddr == null)
            {
                throw new NotSupportedException($"无法在网络接口索引{interfaceIndex}获取SrcAddress");
            }

            this.SrcAddress = srcAddr ?? bestSrcSockAddr.IPAddress;
            this.DstAddress = dstAddr;
            this.InterfaceIndex = interfaceIndex.Value;
            this.IsOutbound = errorCode == 0;
            this.IsLoopback = IPAddress.IsLoopback(dstAddr) && dstAddr.Equals(this.SrcAddress);
        }

        /// <summary>
        /// 是否为any的ip
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static bool IsIPAddressAny(IPAddress? address)
        {
            return address != null && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any));
        }

        /// <summary>
        /// 获取网络接口索引
        /// </summary>
        /// <param name="dstSockAddr"></param>
        /// <returns></returns>
        /// <exception cref="NetworkInformationException"></exception>
        private static int GetInterfaceIndex(ref SockAddress dstSockAddr)
        {
            var errorCode = IPHelpApiNative.GetBestInterfaceEx(ref dstSockAddr, out var ifIdx);
            return errorCode == 0 ? ifIdx : throw new NetworkInformationException(errorCode);
        }

        /// <summary>
        /// 获取网络接口索引
        /// </summary>
        /// <param name="dstAddr">目标地址</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NetworkInformationException"></exception>
        public static int GetInterfaceIndex(IPAddress dstAddr)
        {
            if (IsIPAddressAny(dstAddr))
            {
                throw new ArgumentException($"值不能为{dstAddr}", nameof(dstAddr));
            }

            var dstSockAddr = new SockAddress { IPAddress = dstAddr };
            return GetInterfaceIndex(ref dstSockAddr);
        }

        /// <summary>
        /// 使用路由信息创建WinDivertAddress对象
        /// </summary>
        /// <returns></returns>
        public WinDivertAddress CreateAddress()
        {
            var addr = new WinDivertAddress();
            this.ApplyToAddress(addr);
            return addr;
        }

        /// <summary>
        /// 应用路由信息到指定的WinDivertAddress
        /// 将更改Network->IfIdx、Flags.Outbound和Flags.Loopback
        /// </summary>
        /// <param name="addr"></param>
        public unsafe void ApplyToAddress(WinDivertAddress addr)
        {
            addr.Network->IfIdx = this.InterfaceIndex;

            if (this.IsOutbound)
            {
                addr.Flags |= WinDivertAddressFlag.Outbound;
            }
            else
            {
                addr.Flags &= ~WinDivertAddressFlag.Outbound;
            }

            if (this.IsLoopback)
            {
                addr.Flags |= WinDivertAddressFlag.Loopback;
            }
            else
            {
                addr.Flags &= ~WinDivertAddressFlag.Loopback;
            }
        }
    }
}
