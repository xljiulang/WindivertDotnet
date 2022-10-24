using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using Xunit;

namespace WindivertDotnet.Test
{
    [SupportedOSPlatform("windows")]
    public class WinDivertPacketExtensionsTest
    {
        [Fact]
        public void CloneTest()
        {
            using var packet = new WinDivertPacket();
            packet.GetWriter().Write(Random.Shared.Next());

            using var clone = packet.Clone();
            Assert.False(ReferenceEquals(packet, clone));
            Assert.Equal(packet, clone);
        }

        [Fact]
        public void CopyToTest()
        {
            using var packet = new WinDivertPacket();
            packet.GetWriter().Write(Random.Shared.Next());

            using var dstPacket = new WinDivertPacket(100);
            packet.CopyTo(dstPacket);
            Assert.Equal(packet, dstPacket);

            using var emptyPacket = new WinDivertPacket(0);
            Assert.Throws<ArgumentOutOfRangeException>(() => packet.CopyTo(emptyPacket));
        }

        [Fact]
        [SupportedOSPlatform("windows")]
        public unsafe void IPV4TcpReverseEndPointTest()
        {
            var ipHeader = new IPV4Header
            {
                Version = IPVersion.V4,
                HdrLength = 5,
                Protocol = ProtocolType.Tcp,
                Length = 40,
                SrcAddr = IPAddress.Loopback,
                DstAddr = IPAddress.Parse("1.1.1.1")
            };
            var tcpHeader = new TcpHeader
            {
                HdrLength = 5,
                SrcPort = 1234,
                DstPort = 443
            };

            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.Write(ipHeader);
            writer.Write(tcpHeader);

            var state = packet.ReverseEndPoint();
            var result = packet.GetParseResult();

            Assert.True(state);
            Assert.Equal(ipHeader.SrcAddr, result.IPV4Header->DstAddr);
            Assert.Equal(ipHeader.DstAddr, result.IPV4Header->SrcAddr);
            Assert.Equal(tcpHeader.SrcPort, result.TcpHeader->DstPort);
            Assert.Equal(tcpHeader.DstPort, result.TcpHeader->SrcPort);
        }

        [Fact]
        public unsafe void IPV6UdpReverseEndPointTest()
        {
            var ipHeader = new IPV6Header
            {
                Version = IPVersion.V6,
                NextHdr = ProtocolType.Udp,
                Length = (ushort)(sizeof(UdpHeader)),
                SrcAddr = IPAddress.IPv6Loopback,
                DstAddr = IPAddress.Parse("fe80::3d4d:f188:49a5:36c6%12")
            };
            var udpHeader = new UdpHeader
            {
                Length = 8,
                SrcPort = 1234,
                DstPort = 443
            };

            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.Write(ipHeader);
            writer.Write(udpHeader);

            var state = packet.ReverseEndPoint();
            var result = packet.GetParseResult();

            Assert.True(state);
            Assert.Equal(ipHeader.SrcAddr, result.IPV6Header->DstAddr);
            Assert.Equal(ipHeader.DstAddr, result.IPV6Header->SrcAddr);
            Assert.Equal(udpHeader.SrcPort, result.UdpHeader->DstPort);
            Assert.Equal(udpHeader.DstPort, result.UdpHeader->SrcPort);
        }


        [Fact]
        public unsafe void ApplyLengthToHeadersIPv4Test()
        {
            var ipHeader = new IPV4Header
            {
                Version = IPVersion.V4,
                HdrLength = 5,
            };

            using var packet = new WinDivertPacket();
            packet.GetWriter().Write(ipHeader);
            var count = packet.ApplyLengthToHeaders();
            var result = packet.GetParseResult();
            Assert.Equal(1, count);
            Assert.Equal(packet.Length, result.IPV4Header->Length);

            packet.Length += 10;
            result = packet.GetParseResult();
            count = packet.ApplyLengthToHeaders();
            Assert.Equal(1, count);
            Assert.Equal(packet.Length, result.IPV4Header->Length);
        }

        [Fact]
        public unsafe void ApplyLengthToHeadersIPv4UdpTest()
        {
            var ipHeader = new IPV4Header
            {
                Version = IPVersion.V4,
                HdrLength = 5,
                Protocol = ProtocolType.Udp,
            };

            var udpHeader = new UdpHeader();


            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.Write(ipHeader);
            writer.Write(udpHeader);
            writer.Advance(255);

            var count = packet.ApplyLengthToHeaders();
            var result = packet.GetParseResult();

            Assert.Equal(2, count);
            Assert.Equal(packet.Length, result.IPV4Header->Length);
            Assert.Equal(packet.Length - sizeof(IPV4Header), result.UdpHeader->Length);
        }

        [Fact]
        public unsafe void ApplyLengthToHeadersIPv6Test()
        {
            var ipHeader = new IPV6Header
            {
                Version = IPVersion.V6,
            };

            using var packet = new WinDivertPacket();
            packet.GetWriter().Write(ipHeader);
            var count = packet.ApplyLengthToHeaders();
            var result = packet.GetParseResult();
            Assert.Equal(1, count);
            Assert.Equal(packet.Length - sizeof(IPV6Header), result.IPV6Header->Length);

            packet.Length += 10;
            result = packet.GetParseResult();
            count = packet.ApplyLengthToHeaders();
            Assert.Equal(1, count);
            Assert.Equal(packet.Length - sizeof(IPV6Header), result.IPV6Header->Length);
        }
    }
}
