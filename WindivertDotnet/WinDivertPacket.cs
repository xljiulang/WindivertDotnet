using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的数据包
    /// </summary>
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}")]
    public class WinDivertPacket : SafeHandleZeroOrMinusOneIsInvalid, IEquatable<WinDivertPacket>, ICloneable
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
        public unsafe Span<byte> Span => new(this.handle.ToPointer(), this.length);

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
            : this(MemoryNative.AllocZeroed(capacity), capacity, ownsHandle: true)
        {
            this.Capacity = capacity;
            this.handle = MemoryNative.AllocZeroed(capacity);
        }

        /// <summary>
        /// WinDivert的数据包
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="capacity"></param>
        /// <param name="ownsHandle"></param>
        private unsafe WinDivertPacket(IntPtr handle, int capacity, bool ownsHandle)
            : base(ownsHandle)
        {
            this.handle = handle;
            this.Capacity = capacity;
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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public unsafe Span<byte> GetSpan(int offset, int count)
        {
            var pointer = this.GetPointer(offset, count);
            return new Span<byte>(pointer, count);
        }

        /// <summary>
        /// 切片
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="count">大小</param>
        /// <returns></returns>
        public unsafe WinDivertPacket Slice(int offset, int count)
        {
            var pointer = this.GetPointer(offset, count);
            var handle = new IntPtr(pointer);
            return new WinDivertPacket(handle, count, ownsHandle: false)
            {
                length = count
            };
        }


        /// <summary>
        /// 获取指针
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private unsafe void* GetPointer(int offset, int count)
        {
            if (offset < 0 || offset > this.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0 || this.Capacity - offset < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return (this.handle + offset).ToPointer();
        }

        /// <summary>
        /// 重新计算和修改相关的Checksums
        /// 当修改数据包之后需要重新计算
        /// </summary>
        /// <param name="addr">地址信息</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
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
        [SupportedOSPlatform("windows")]
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
        [SupportedOSPlatform("windows")]
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
                var ptr = this.handle.ToPointer();
                var version = (IPVersion)(Unsafe.Read<byte>(ptr) >> 4);

                if (version == IPVersion.V4 && this.length >= sizeof(IPV4Header))
                {
                    var header = (IPV4Header*)ptr;
                    srcAddr = header->SrcAddr;
                    dstAddr = header->DstAddr;
                    return true;
                }

                if (version == IPVersion.V6 && this.length >= sizeof(IPV6Header))
                {
                    var header = (IPV6Header*)ptr;
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
        [SupportedOSPlatform("windows")]
        public bool DecrementTTL()
        {
            return WinDivertNative.WinDivertHelperDecrementTTL(this, this.length);
        }

        /// <summary>
        /// 获取包的哈希
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public override int GetHashCode()
        {
            return this.GetHashCode(seed: 0L);
        }

        /// <summary>
        /// 获取包的哈希
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public int GetHashCode(long seed)
        {
            return WinDivertNative.WinDivertHelperHashPacket(this, this.length, seed);
        }

        /// <summary>
        /// 获取缓冲区的Writer对象
        /// 该对象在写入数据后自动影响Length属性
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public WindivertBufferWriter GetWriter(int offset = 0)
        {
            return new WindivertBufferWriter(this, offset);
        }

        /// <summary>
        /// 获取包的解析结果
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
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

        /// <summary>
        /// 克隆
        /// </summary>  
        /// <returns></returns>
        public WinDivertPacket Clone()
        {
            var target = new WinDivertPacket(this.Capacity);
            this.CopyTo(target);
            return target;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// 复制数据到指定目标
        /// </summary> 
        /// <param name="target"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CopyTo(WinDivertPacket target)
        {
            target.GetWriter().Write(this.Span);
        }

        /// <summary>
        /// 翻转Src和Dst地址和端口
        /// </summary> 
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public unsafe bool ReverseEndPoint()
        {
            var result = this.GetParseResult();
            if (result.IPV4Header != null)
            {
                var src = result.IPV4Header->SrcAddr;
                result.IPV4Header->SrcAddr = result.IPV4Header->DstAddr;
                result.IPV4Header->DstAddr = src;
            }
            else if (result.IPV6Header != null)
            {
                var src = result.IPV6Header->SrcAddr;
                result.IPV6Header->SrcAddr = result.IPV6Header->DstAddr;
                result.IPV6Header->DstAddr = src;
            }
            else
            {
                return false;
            }

            if (result.TcpHeader != null)
            {
                var src = result.TcpHeader->SrcPort;
                result.TcpHeader->SrcPort = result.TcpHeader->DstPort;
                result.TcpHeader->DstPort = src;
            }

            if (result.UdpHeader != null)
            {
                var src = result.UdpHeader->SrcPort;
                result.UdpHeader->SrcPort = result.UdpHeader->DstPort;
                result.UdpHeader->DstPort = src;
            }

            return true;
        }

        /// <summary>
        /// 应用当前的Length值到IP头和Udp头
        /// 返回影响到Header数
        /// </summary> 
        /// <returns></returns>
        public unsafe int ApplyLengthToHeaders()
        {
            if (this.length < sizeof(IPV4Header))
            {
                return 0;
            }

            var count = 0;
            var ptr = (byte*)this.handle.ToPointer();
            var version = (IPVersion)(Unsafe.Read<byte>(ptr) >> 4);

            ProtocolType protocol;
            int ipHeaderLength;

            if (version == IPVersion.V4)
            {
                var header = (IPV4Header*)ptr;
                header->Length = (ushort)this.length;
                protocol = header->Protocol;
                ipHeaderLength = header->HdrLength * 4;
                count += 1;
            }
            else if (version == IPVersion.V6 && this.length >= sizeof(IPV6Header))
            {
                var header = (IPV6Header*)ptr;
                header->Length = (ushort)(this.length - sizeof(IPV6Header));
                protocol = header->NextHdr;
                ipHeaderLength = sizeof(IPV6Header);
                count += 1;
            }
            else
            {
                return count;
            }

            if (protocol == ProtocolType.Udp &&
                this.length >= ipHeaderLength + sizeof(UdpHeader))
            {
                var header = (UdpHeader*)(ptr + ipHeaderLength);
                header->Length = (ushort)(this.length - ipHeaderLength);
                count += 1;
            }

            return count;
        }



        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns> 
        public bool Equals(WinDivertPacket? other)
        {
            return other != null &&
                 other.length == this.length &&
                 other.Span.SequenceEqual(this.Span);
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as WinDivertPacket);
        }
    }
}
