using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    [DebuggerDisplay("Length = {Length}")]
    public class WinDivertPacket : IDisposable
    {
        private readonly WinDivertPacketHandle handle;

        public int Capacity { get; }

        public int Length { get; set; }

        public SafeHandle Handle => this.handle;

        public Span<byte> Span => this.handle.GetSpan(this.Length);

        public WinDivertPacket(int capacity = ushort.MaxValue)
        {
            this.Capacity = capacity;
            this.handle = new WinDivertPacketHandle(capacity);
        }

        public void Dispose()
        {
            this.handle.Dispose();
        }

        public bool CalcChecksums(ref WinDivertAddress addr, ChecksumsFlag flag = ChecksumsFlag.All)
        {
            return WinDivertNative.WinDivertHelperCalcChecksums(this.handle, this.Length, ref addr, flag);
        }

        public bool DecrementTTL()
        {
            return WinDivertNative.WinDivertHelperDecrementTTL(this.handle, this.Length);
        }

        public int GetHash(long seed = 0)
        {
            return WinDivertNative.WinDivertHelperHashPacket(this.handle, this.Length, seed);
        }

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
