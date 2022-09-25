using System;

namespace WindivertDotnet
{
    /// <summary>
    /// 关闭内容
    /// </summary>
    [Flags]
    public enum WinDivertShutdown
    {
        /// <summary>
        /// 接收
        /// </summary>
        Recv = 0x1,

        /// <summary>
        /// 发送
        /// </summary>
        Send = 0x2,

        /// <summary>
        /// 接收和发送
        /// </summary>
        Both = Recv | Send
    }
}
