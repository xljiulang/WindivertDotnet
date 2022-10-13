namespace WindivertDotnet
{
    /// <summary>
    /// 工作层
    /// </summary>
    public enum WinDivertLayer : byte
    {
        /// <summary>
        /// 传入/传出本地计算机的网络数据包
        /// </summary>
        Network = 0,

        /// <summary>
        /// 通过本地计算机的网络数据包
        /// </summary>
        Forward = 1,

        /// <summary>
        /// 网络流已建立/已删除事件
        /// </summary>
        Flow = 2,

        /// <summary>
        /// 套接字操作事件
        /// </summary>
        Socket = 3,

        /// <summary>
        /// WinDivert处理事件
        /// </summary>
        Reflect = 4,
    }
}
