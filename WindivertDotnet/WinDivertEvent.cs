namespace WindivertDotnet
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum WinDivertEvent : byte
    {
        /// <summary>
        /// 新的网络数据包
        /// </summary>
        NetworkPacket = 0,

        /// <summary>
        /// 将创建一个新流
        /// </summary>
        FlowEstablished = 1,

        /// <summary>
        /// 旧流将被删除
        /// </summary>
        FlowDeleted = 2,

        /// <summary>
        /// bind操作
        /// </summary>
        SocketBind = 3,

        /// <summary>
        /// connect操作
        /// </summary>
        SocketConnect = 4,

        /// <summary>
        /// listen操作
        /// </summary>
        SocketListen = 5,

        /// <summary>
        /// accept操作
        /// </summary>
        SocketAccept = 6,

        /// <summary>
        /// 套接字终结点已关闭
        /// </summary>
        SocketClose = 7,

        /// <summary>
        /// 打开了新的WinDivert实例
        /// </summary>
        ReflectOpen = 8,

        /// <summary>
        /// 旧的WinDivert实例被关闭
        /// </summary>
        ReflectClose = 9,
    }
}
