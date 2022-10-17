using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的数据包
    /// </summary>
    [SupportedOSPlatform("windows")]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}")]
    public class WinDivertPacket : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// MTU的最大长度
        /// </summary>
        public const int MTU_MAX = 40 + 0xFFFF;

        /// <summary>
        /// 有效数据的长度
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int length;

        /// <summary>
        /// 获取缓冲区容量
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// 获取有效数据视图
        /// </summary>
        public Span<byte> Span => this.GetSpan(0, this.length);

        /// <summary>
        /// 获取或设置有效数据的长度
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Length
        {
            get => this.length;
            set
            {
                if (value < 0 || value > this.Capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(Length));
                }
                this.length = value;
            }
        }

        /// <summary>
        /// WinDivert的数据包
        /// </summary>
        /// <param name="capacity">最大容量</param> 
        public WinDivertPacket(int capacity = MTU_MAX)
            : base(ownsHandle: true)
        {
            this.Capacity = capacity;
            this.handle = MemoryNative.AllocZeroed(capacity);
        }

        /// <summary>
        /// 释放本机句柄
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            MemoryNative.Free(this.handle);
            return true;
        }

        /// <summary>
        /// 将有效数据清0
        /// </summary>
        public void Clear()
        {
            this.Span.Clear();
        }

        /// <summary>
        /// 获取缓冲区的Span
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="count">字节数</param>
        /// <returns></returns>
        public unsafe Span<byte> GetSpan(int offset, int count)
        {
            if (offset < 0 || offset > this.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0 || this.Capacity - offset < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var pointer = (byte*)this.handle.ToPointer() + offset;
            return new Span<byte>(pointer, count);
        }

        /// <summary>
        /// 重新计算和修改相关的Checksums
        /// 当修改数据包之后需要重新计算
        /// </summary>
        /// <param name="addr">地址信息</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool CalcChecksums(WinDivertAddress addr, ChecksumsFlag flag = ChecksumsFlag.All)
        {
            return WinDivertNative.WinDivertHelperCalcChecksums(this, this.length, addr, flag);
        }

        /// <summary>
        /// 根据IP地址重新计算和修改addr的Network->IfIdx
        /// 当修改源IP或目标IP(不含对调)之后需要重新计算
        /// </summary>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="NetworkInformationException"></exception>
        public unsafe bool CalcNetworkIfIdx(WinDivertAddress addr)
        {
            if (addr.Layer == WinDivertLayer.Network &&
                this.TryParseIPAddress(out _, out var dstAddr))
            {
                addr.Network->IfIdx = WinDivertRouter.GetInterfaceIndex(dstAddr);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 根据IP地址和addr.Network->IfIdx重新计算和修改addr的Outbound标记
        /// 当修改源IP或目标IP之后需要重新计算
        /// </summary>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        public unsafe bool CalcOutboundFlag(WinDivertAddress addr)
        {
            if (addr.Layer != WinDivertLayer.Network ||
                this.TryParseIPAddress(out var srcAddr, out var dstAddr) == false)
            {
                return false;
            }

            var router = new WinDivertRouter(dstAddr, srcAddr, addr.Network->IfIdx);
            if (router.IsOutbound == true)
            {
                addr.Flags |= WinDivertAddressFlag.Outbound;
            }
            else
            {
                addr.Flags &= ~WinDivertAddressFlag.Outbound;
            }
            return true;
        }

        /// <summary>
        /// 根据IP地址重新计算和修改addr的Loopback标记
        /// 当修改源IP或目标IP(不含对调)之后需要重新计算
        /// </summary> 
        /// <param name="addr">地址信息</param> 
        /// <returns></returns>
        public bool CalcLoopbackFlag(WinDivertAddress addr)
        {
            if (this.TryParseIPAddress(out var srcAddr, out var dstAddr))
            {
                if (IPAddress.IsLoopback(srcAddr) && srcAddr.Equals(dstAddr))
                {
                    addr.Flags |= WinDivertAddressFlag.Loopback;
                }
                else
                {
                    addr.Flags &= ~WinDivertAddressFlag.Loopback;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试解析IP地址
        /// </summary>
        /// <param name="srcAddr"></param>
        /// <param name="dstAddr"></param>
        /// <returns></returns>
        private unsafe bool TryParseIPAddress(
            [MaybeNullWhen(false)] out IPAddress srcAddr,
            [MaybeNullWhen(false)] out IPAddress dstAddr)
        {
            if (this.length > 1)
            {
                var version = this.Span[0] >> 4;
                if (version == 4 && this.length >= sizeof(IPV4Header))
                {
                    var header = (IPV4Header*)this.handle.ToPointer();
                    srcAddr = header->SrcAddr;
                    dstAddr = header->DstAddr;
                    return true;
                }

                if (version == 6 && this.length >= sizeof(IPV6Header))
                {
                    var header = (IPV6Header*)this.handle.ToPointer();
                    srcAddr = header->SrcAddr;
                    dstAddr = header->DstAddr;
                    return true;
                }
            }

            srcAddr = default;
            dstAddr = default;
            return false;
        }

        /// <summary>
        /// ttl减1
        /// </summary>
        /// <returns></returns>
        public bool DecrementTTL()
        {
            return WinDivertNative.WinDivertHelperDecrementTTL(this, this.length);
        }

        /// <summary>
        /// 获取包的哈希
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.GetHashCode(seed: 0L);
        }

        /// <summary>
        /// 获取包的哈希
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public int GetHashCode(long seed)
        {
            return WinDivertNative.WinDivertHelperHashPacket(this, this.length, seed);
        }

        /// <summary>
        /// 获取包的解析结果
        /// </summary>
        /// <returns></returns>
        public unsafe WinDivertParseResult GetParseResult()
        {
            IPV4Header* pIPV4Header;
            IPV6Header* pIPV6Header;
            IcmpV4Header* pIcmpV4Header;
            IcmpV6Header* pIcmpV6Header;
            byte protocol;
            TcpHeader* pTcpHeader;
            UdpHeader* pUdpHeader;
            byte* pData;
            int dataLength;
            byte* pNext;
            int nextLength;

            WinDivertNative.WinDivertHelperParsePacket(
                this,
                this.length,
                &pIPV4Header,
                &pIPV6Header,
                &protocol,
                &pIcmpV4Header,
                &pIcmpV6Header,
                &pTcpHeader,
                &pUdpHeader,
                &pData,
                &dataLength,
                &pNext,
                &nextLength);

            return new WinDivertParseResult
            {
                IPV4Header = pIPV4Header,
                IPV6Header = pIPV6Header,
                Protocol = (ProtocolType)protocol,
                IcmpV4Header = pIcmpV4Header,
                IcmpV6Header = pIcmpV6Header,
                TcpHeader = pTcpHeader,
                UdpHeader = pUdpHeader,
                Data = pData,
                DataLength = dataLength,
                Next = pNext,
                NextLength = nextLength
            };
        }
    }
}
