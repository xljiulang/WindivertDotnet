using System;

namespace WindivertDotnet
{
    /// <summary>
    /// tcp标记位
    /// </summary>
    [Flags]
    public enum TcpFlag : byte
    {
        /// <summary>
        /// 结束位
        /// </summary>
        Fin = 0x1,

        /// <summary>
        /// 请求位
        /// </summary>
        Syn = 0x2,

        /// <summary>
        /// 重置位
        /// </summary>
        Rst = 0x4,

        /// <summary>
        /// 推送位
        /// </summary>
        Psh = 0x8,

        /// <summary>
        /// 确认位
        /// </summary>
        Ack = 0x10,

        /// <summary>
        /// 紧急位
        /// </summary>
        Urg = 0x20,
    }
}
