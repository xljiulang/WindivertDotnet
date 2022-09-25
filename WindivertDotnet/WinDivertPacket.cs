using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的数据包
    /// </summary>
    [DebuggerDisplay("Length = {Length}")]
    public sealed class WinDivertPacket : IDisposable
    {
        private readonly WinDivertPacketHandle handle;

        /// <summary>
        /// 获取最大容量
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// 获取或设置有效数据的长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 获取关联的句柄
        /// </summary>
        public SafeHandle Handle => this.handle;

        /// <summary>
        /// 获取数据视图
        /// </summary>
        public Span<byte> Span => this.handle.GetSpan(this.Length);

        /// <summary>
        /// WinDivert的数据包
        /// </summary>
        /// <param name="capacity">最大容量</param>
        public WinDivertPacket(int capacity = ushort.MaxValue)
        {
            this.Capacity = capacity;
            this.handle = new WinDivertPacketHandle(capacity);
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose()
        {
            this.handle.Dispose();
        }

        /// <summary>
        /// 重新计算和修改相关的Checksums
        /// </summary>
        /// <param name="addr">地址信息</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool CalcChecksums(ref WinDivertAddress addr, ChecksumsFlag flag = ChecksumsFlag.All)
        {
            return WinDivertNative.WinDivertHelperCalcChecksums(this.handle, this.Length, ref addr, flag);
        }

        /// <summary>
        /// ttl减1
        /// </summary>
        /// <returns></returns>
        public bool DecrementTTL()
        {
            return WinDivertNative.WinDivertHelperDecrementTTL(this.handle, this.Length);
        }

        /// <summary>
        /// 获取包的哈希
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public int GetHash(long seed = 0)
        {
            return WinDivertNative.WinDivertHelperHashPacket(this.handle, this.Length, seed);
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
                this.handle,
                this.Length,
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
