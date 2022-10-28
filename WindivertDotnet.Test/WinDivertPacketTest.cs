using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using Xunit;

namespace WindivertDotnet.Test
{
    [SupportedOSPlatform("windows")]
    public class WinDivertPacketTest
    {
        [Fact]
        public void CtorTest()
        {
            using var packet = new WinDivertPacket(10);
            Assert.Equal(10, packet.Capacity);
            Assert.Equal(0, packet.Length);
            Assert.Equal(0, packet.Span.Length);
        }

        [Fact]
        public void LengthTest()
        {
            using var packet = new WinDivertPacket(10);
            packet.Length = 5;
            Assert.Equal(10, packet.Capacity);
            Assert.Equal(5, packet.Length);
            Assert.Equal(5, packet.Span.Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => packet.Length = 12);
        }

        [Fact]
        public void GetSpanTest()
        {
            using var packet = new WinDivertPacket(10);
            packet.Length = 1;
            Assert.Equal(0, packet.GetSpan(0, 10)[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => packet.GetSpan(1, 10));
        }


        [Fact]
        public void SliceTest()
        {
            using var packet = new WinDivertPacket(10);
            var slicePacket = packet.Slice(2, 8);

            var span = packet.GetSpan(0, packet.Capacity);
            span[1] = 1;
            span[2] = 2;
            span[4] = 4;
            span[8] = 8;
           
            Assert.Equal(8, slicePacket.Capacity);
            Assert.Equal(8, slicePacket.Length);
            Assert.True(span.Slice(2, 8).SequenceEqual(slicePacket.Span));
        }

        [Fact]
        public unsafe void CalcNetworkIfIdxTest()
        {
            var router = new WinDivertRouter(IPAddress.Loopback);
            using var addr = new WinDivertAddress();

            using var packet = new WinDivertPacket();
            var ipv4Header = CreateIPV4Header(router);
            packet.GetWriter().Write(ipv4Header);

            packet.CalcNetworkIfIdx(addr);
            Assert.Equal(router.InterfaceIndex, addr.Network->IfIdx);
        }

        [Fact]
        public void CalcLoopbackFlagTest()
        {
            var router = new WinDivertRouter(IPAddress.Loopback);
            using var addr = new WinDivertAddress();

            using var packet = new WinDivertPacket();
            var ipv4Header = CreateIPV4Header(router);
            packet.GetWriter().Write(ipv4Header);

            packet.CalcLoopbackFlag(addr);
            Assert.Equal(router.IsLoopback, addr.Flags.HasFlag(WinDivertAddressFlag.Loopback));
        }


        [Fact]
        public void CalcOutboundFlagTest()
        {
            var router = new WinDivertRouter(IPAddress.Loopback);
            using var addr = new WinDivertAddress();

            using var packet = new WinDivertPacket();
            var ipv4Header = CreateIPV4Header(router);
            packet.GetWriter().Write(ipv4Header);

            packet.CalcOutboundFlag(addr);
            Assert.Equal(router.IsOutbound, addr.Flags.HasFlag(WinDivertAddressFlag.Outbound));
        }

        [Fact]
        public void EqualsTest()
        {
            using var p1 = new WinDivertPacket();
            using var p2 = new WinDivertPacket();
            p1.GetWriter().Write(1);
            p2.GetWriter().Write(1);

            Assert.Equal(p1, p2);

            p1.GetWriter().Write(3);
            p2.GetWriter().Write(4);

            Assert.NotEqual(p1, p2);
        }


        private static IPV4Header CreateIPV4Header(WinDivertRouter router)
        {
            return new IPV4Header
            {
                Version = IPVersion.V4,
                HdrLength = 5,
                Length = 20,
                Protocol = ProtocolType.IP,
                SrcAddr = router.SrcAddress,
                DstAddr = router.DstAddress
            };
        }
    }
}
