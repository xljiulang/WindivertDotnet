using System;
using System.Threading;

namespace WindivertDotnet
{
    /// <summary>
    /// id或序列号生成器
    /// </summary>
    public sealed class IdSeqNum
    {
        private static readonly Random random = new();
        private int value = random.Next();

        /// <summary>
        /// 获取公共实例
        /// </summary>
        public static IdSeqNum Shared { get; } = new IdSeqNum();

        /// <summary>
        /// 生成下一个UInt16值
        /// </summary>
        /// <returns></returns>
        public ushort NextUInt16() => (ushort)Interlocked.Increment(ref value);

        /// <summary>
        /// 生成下一个UInt32值
        /// </summary>
        /// <returns></returns>
        public ushort NextUInt32() => (ushort)Interlocked.Increment(ref value);
    }
}
