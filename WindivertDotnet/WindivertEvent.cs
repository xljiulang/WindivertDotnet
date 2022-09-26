namespace WindivertDotnet
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum WinDivertEvent : byte
    {
        NetworkPacket = 0,
        FlowEstablished = 1,
        FlowDeleted = 2,
        SocketBind = 3,
        SocketConnect = 4,
        SocketListen = 5,
        SocketAccept = 6,
        SocketClose = 7,
        ReflectOpen = 8,
        ReflectClose = 9,
    }
}
