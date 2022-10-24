using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace WindivertDotnet
{
    /// <summary>
    /// WinDivertPacket扩展
    /// </summary>
    public static class WinDivertPacketExtensions
    {
        /// <summary>
        /// 克隆自制
        /// </summary>
        /// <param name="packet"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static WinDivertPacket Clone(this WinDivertPacket packet)
        {
            var dstPacket = new WinDivertPacket(packet.Capacity);
            packet.CopyTo(dstPacket);
            return dstPacket;
        }

        /// <summary>
        /// 复制数据到dstPacket
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="dstPacket"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void CopyTo(this WinDivertPacket packet, WinDivertPacket dstPacket)
        {
            dstPacket.GetWriter().Write(packet.Span);
        }

        /// <summary>
        /// 应用当前的Length值到IP头和Udp头
        /// 返回影响到Header数
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public unsafe static int ApplyLengthToHeaders(this WinDivertPacket packet)
        {
            if (packet.Length < sizeof(IPV4Header))
            {
                return 0;
            }

            var count = 0;
            var ptr = (byte*)packet.DangerousGetHandle().ToPointer();
            var version = Unsafe.Read<byte>(ptr) >> 4;

            ProtocolType protocol;
            int ipHeaderLength;

            if (version == 4)
            {
                var header = (IPV4Header*)ptr;
                header->Length = (ushort)packet.Length;
                protocol = header->Protocol;
                ipHeaderLength = header->HdrLength * 4;
                count += 1;
            }
            else if (version == 6 && packet.Length >= sizeof(IPV6Header))
            {
                var header = (IPV6Header*)ptr;
                header->Length = (ushort)(packet.Length - sizeof(IPV6Header));
                protocol = header->NextHdr;
                ipHeaderLength = sizeof(IPV6Header);
                count += 1;
            }
            else
            {
                return count;
            }

            if (protocol == ProtocolType.Udp &&
                packet.Length >= ipHeaderLength + sizeof(UdpHeader))
            {
                var header = (UdpHeader*)(ptr + ipHeaderLength);
                header->Length = (ushort)(packet.Length - ipHeaderLength);
                count += 1;
            }

            return count;
        }

        /// <summary>
        /// 翻转Src和Dst地址和端口
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public unsafe static bool ReverseEndPoint(this WinDivertPacket packet)
        {
            var result = packet.GetParseResult();
            if (result.IPV4Header != null)
            {
                var src = result.IPV4Header->SrcAddr;
                result.IPV4Header->SrcAddr = result.IPV4Header->DstAddr;
                result.IPV4Header->DstAddr = src;
            }
            else if (result.IPV6Header != null)
            {
                var src = result.IPV6Header->SrcAddr;
                result.IPV6Header->SrcAddr = result.IPV6Header->DstAddr;
                result.IPV6Header->DstAddr = src;
            }
            else
            {
                return false;
            }

            if (result.TcpHeader != null)
            {
                var src = result.TcpHeader->SrcPort;
                result.TcpHeader->SrcPort = result.TcpHeader->DstPort;
                result.TcpHeader->DstPort = src;
            }

            if (result.UdpHeader != null)
            {
                var src = result.UdpHeader->SrcPort;
                result.UdpHeader->SrcPort = result.UdpHeader->DstPort;
                result.UdpHeader->DstPort = src;
            }

            return true;
        }
    }
}
