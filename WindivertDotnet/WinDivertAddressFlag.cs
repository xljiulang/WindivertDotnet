using System;

namespace WindivertDotnet
{
    /// <summary>
    /// WinDivertAddress标记
    /// </summary>
    [Flags]
    public enum WinDivertAddressFlag : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 嗅探事件模式
        /// </summary>
        Sniffed = 0x1,

        /// <summary>
        /// 出站数据包
        /// </summary>
        Outbound = 0x2,

        /// <summary>
        /// 环回数据包
        /// </summary>
        Loopback = 0x4,

        /// <summary>
        /// 冒名顶替数据包
        /// </summary>
        Impostor = 0x8,

        /// <summary>
        /// IPv6 数据包
        /// </summary>
        IPv6 = 0x10,

        /// <summary>
        /// IPv4 校验和有效
        /// </summary>
        IPChecksum = 0x20,

        /// <summary>
        /// tcp 校验和有效
        /// </summary>
        TCPChecksum = 0x40,

        /// <summary>
        /// udp 校验和有效
        /// </summary>
        UDPChecksum = 0x80
    }
}
