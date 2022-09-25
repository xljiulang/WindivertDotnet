using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    public class WinDivertPacket : IDisposable
    {
        public int Capacity { get; }

        public IntPtr Handle { get; }

        public int Length { get; set; }

        public Span<byte> Span => this.GetSpan(this.Length);

        public unsafe Span<byte> GetSpan(int length)
        {
            return length > this.Capacity
                ? throw new ArgumentOutOfRangeException(nameof(length))
                : new Span<byte>(this.Handle.ToPointer(), length);
        }

        public WinDivertPacket(int capacity = ushort.MaxValue)
        {
            this.Capacity = capacity;
            this.Handle = Marshal.AllocHGlobal(capacity);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(this.Handle);
        }

        public bool CalcChecksums(ref WinDivertAddress addr, ChecksumsFlag flag = ChecksumsFlag.All)
        {
            return WinDivertNative.WinDivertHelperCalcChecksums(this.Handle, this.Length, ref addr, flag);
        }

        public bool DecrementTTL()
        {
            return WinDivertNative.WinDivertHelperDecrementTTL(this.Handle, this.Length);
        }

        public int GetHash(long seed = 0)
        {
            return WinDivertNative.WinDivertHelperHashPacket(this.Handle, this.Length, seed);
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
                this.Handle,
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
