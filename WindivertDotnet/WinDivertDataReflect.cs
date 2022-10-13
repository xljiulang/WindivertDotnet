namespace WindivertDotnet
{
    /// <summary>
    /// WinDivert信息
    /// </summary>
    public struct WinDivertDataReflect
    {
        /// <summary>
        /// WinDivert句柄打开时间的时间戳
        /// </summary>
        public long Timestamp;

        /// <summary>
        /// WinDivert打开句柄的进程的ID
        /// </summary>
        public int ProcessId;

        /// <summary>
        /// WinDivert工作层
        /// </summary>
        public WinDivertLayer Layer;

        /// <summary>
        /// WinDivert的Flags
        /// </summary>
        public WinDivertFlag Flags;

        /// <summary>
        /// WinDivert的优先级
        /// </summary>
        public short Priority;
    }
}
