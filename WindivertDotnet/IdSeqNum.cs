using System;
using System.Threading;

namespace WindivertDotnet
{
    /// <summary>
    /// 提供Id和序列号生成
    /// </summary>
    public static class IdSeqNum
    {
        private static readonly Random random = new();
        private static int id = random.Next();
        private static int seqNum = random.Next();

        /// <summary>
        /// 获取新的16位Id
        /// </summary>
        public static ushort IdUInt16() => (ushort)Interlocked.Increment(ref id);

        /// <summary>
        /// 获取新的32位id
        /// </summary>
        public static uint IdUInt32() => (uint)Interlocked.Increment(ref id);

        /// <summary>
        /// 获取新的16位SeqNum
        /// </summary>
        public static ushort SeqNumUInt16() => (ushort)Interlocked.Increment(ref seqNum);

        /// <summary>
        /// 获取新的32位SeqNum
        /// </summary>
        public static uint SeqNumUInt32() => (uint)Interlocked.Increment(ref seqNum);
    }
}
