namespace WindivertDotnet
{
    /// <summary>
    /// WinDivert方式标志
    /// </summary>
    public enum WinDivertFlag : ulong
    {
        ///<summary>
        ///不指定
        ///</summary>
        None = 0,

        ///<summary>
        ///此标志在数据包嗅探模式下打开WinDivert句柄
        ///在数据包嗅探模式下，原始数据包不会被丢弃和转移（默认值），而是被复制和转移
        ///此模式对于实现类似于当前使用的应用程序的数据包探查工具非常有用
        ///</summary>
        Sniff = 0x0001,

        ///<summary>
        ///此标志指示用户应用程序不打算使用Recv相关方法读取匹配的数据包，而应以静默方式丢弃这些数据包
        ///这对于使用WinDivert筛选器语言实现简单的数据包筛选器非常有用
        ///</summary>
        Drop = 0x0002,

        ///<summary>
        ///此标志强制句柄进入仅接收模式，从而有效地禁用Send相关方法的功能
        ///这意味着可以阻止/捕获数据包或事件，但不能注入它们
        ///</summary>
        RecvOnly = 0x0004,

        ///<summary>
        ///等同于RecvOnly
        ///</summary>
        ReadOnly = RecvOnly,

        ///<summary>
        ///此标志强制句柄进入仅发送模式，从而有效地禁用Recv相关方法
        ///这意味着可以注入数据包或事件，但不能阻止/捕获它们
        ///</summary>
        SendOnly = 0x0008,

        ///<summary>
        ///等同于SendOnly
        ///</summary>
        WriteOnly = SendOnly,

        ///<summary>
        ///如果尚未安装WinDivert驱动程序，开启则此标志会产生对应的异常
        ///</summary>
        NoInstall = 0x0010,

        ///<summary>
        ///开启此标记，句柄将捕获入站IP片段，但不会捕获入站重新组合的IP数据包
        ///否则句柄将捕获入站重新组合的IP数据包，但不会捕获入站IP片段
        ///</summary>
        Fragments = 0x0020,
    }
}
