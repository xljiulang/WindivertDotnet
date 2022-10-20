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


        private static IPV4Header CreateIPV4Header(WinDivertRouter router)
        {
            return new IPV4Header
            {
                Version = 4,
                HdrLength = 5,
                Length = 20,
                Protocol = ProtocolType.IP,
                SrcAddr = router.SrcAddress,
                DstAddr = router.DstAddress
            };
        }
    }
}
