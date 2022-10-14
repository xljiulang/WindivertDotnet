using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的数据包
    /// </summary>
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}")]
    public class WinDivertPacket : SafeHandleZeroOrMinusOneIsInvalid
    {
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
        public WinDivertPacket(int capacity = 0xFFFF + 40)
            : base(ownsHandle: true)
        {
            this.Capacity = capacity;
            this.handle = Marshal.AllocHGlobal(capacity);
            this.Clear();
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.GetSpan(0, this.Capacity).Clear();
        }

        /// <summary>
        /// 创建缓冲区写入对象
        /// </summary>
        /// <param name="offset">缓冲区偏移量</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public IBufferWriter<byte> CreateBufferWriter(int offset = 0)
        {
            return new BufferWriter(this, offset);
        }

        /// <summary>
        /// 释放本机句柄
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(this.handle);
            return true;
        }

        /// <summary>
        /// 获取span
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="sizeHint"></param>
        /// <returns></returns>
        private unsafe Span<byte> GetSpan(int offset, int sizeHint)
        {
            if (offset > this.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (this.Capacity - offset < sizeHint)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            var pointer = (byte*)this.handle.ToPointer() + offset;
            return new Span<byte>(pointer, sizeHint);
        }

        /// <summary>
        /// 重新计算和修改相关的Checksums
        /// </summary>
        /// <param name="addr">地址信息</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool CalcChecksums(WinDivertAddress addr, ChecksumsFlag flag = ChecksumsFlag.All)
        {
            return WinDivertNative.WinDivertHelperCalcChecksums(this, this.length, addr, flag);
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
        /// <exception cref="Win32Exception"></exception>
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

            var flag = WinDivertNative.WinDivertHelperParsePacket(
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

            if (flag == false)
            {
                throw new Win32Exception();
            }

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


        private class BufferWriter : IBufferWriter<byte>
        {
            private int index;
            private readonly WinDivertPacket packet;

            public BufferWriter(WinDivertPacket packet, int offset)
            {
                if (offset >= packet.Capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset));
                }

                this.index = offset;
                this.packet = packet;
            }

            public void Advance(int count)
            {
                this.index += count;
                this.packet.Length = this.index;
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                if (sizeHint == 0)
                {
                    sizeHint = this.packet.Capacity - this.index;
                }
                return this.packet.GetSpan(this.index, sizeHint);
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                throw new NotSupportedException();
            }
        }
    }
}
