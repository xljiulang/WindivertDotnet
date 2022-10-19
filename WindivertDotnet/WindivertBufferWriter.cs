using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// Windivert缓冲区Writer
    /// </summary> 
    public class WindivertBufferWriter : IBufferWriter<byte>
    {
        private int index;
        private readonly WinDivertPacket packet;

        /// <summary>
        /// Windivert缓冲区Writer
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="offset">偏移量</param>
        public WindivertBufferWriter(WinDivertPacket packet, int offset)
        {
            this.packet = packet;
            this.index = offset;
        }

        /// <summary>
        /// 将值写入并翻转字节顺序
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value">翻转之前的值</param>
        public unsafe void WriteReverse<TValue>(TValue value) where TValue : unmanaged
        {
            var span = this.GetSpan(sizeof(TValue))[..sizeof(TValue)];
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
            span.Reverse();
            this.Advance(sizeof(TValue));
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value">值</param>
        public unsafe void Write<TValue>(TValue value) where TValue : unmanaged
        {
            var span = this.GetSpan(sizeof(TValue));
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
            this.Advance(sizeof(TValue));
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Write(ReadOnlySpan<byte> value)
        {
            value.CopyTo(this.GetSpan(value.Length));
            this.Advance(value.Length);
        }

        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Write(byte value)
        {
            const int count = 1;
            this.GetSpan(count)[0] = value;
            this.Advance(count);
        }

        /// <summary>
        /// 向前推进
        /// </summary>
        /// <param name="count">字节数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Advance(int count)
        {
            var size = this.index + count;
            this.packet.Length = size;
            this.index = size;
        }

        /// <summary>
        /// 获取预期大小的写入缓冲区
        /// </summary>
        /// <param name="sizeHint">预期大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            if (sizeHint <= 0)
            {
                sizeHint = this.packet.Capacity - this.index;
            }
            return this.packet.GetSpan(this.index, sizeHint);
        }

        /// <summary>
        /// 获取预期大小的写入缓冲区
        /// </summary>
        /// <param name="sizeHint"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
        {
            throw new NotSupportedException();
        }
    }
}
