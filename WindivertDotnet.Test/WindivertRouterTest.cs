using System.Net;
using System.Runtime.Versioning;
using Xunit;

namespace WindivertDotnet.Test
{
    [SupportedOSPlatform("windows")]
    public class WindivertRouterTest
    {
        [Fact]
        public void IPv4LoopbackTest()
        {
            var router = new WinDivertRouter(IPAddress.Loopback);
            Assert.Equal(IPAddress.Loopback, router.SrcAddress);
            Assert.Equal(IPAddress.Loopback, router.DstAddress);
            Assert.True(router.IsOutbound);
            Assert.True(router.IsLoopback);
        }

        [Fact]
        public unsafe void IPv4LoopbackAddrTest()
        {
            var router = new WinDivertRouter(IPAddress.Loopback);
            using var addr = router.CreateAddress();

            Assert.Equal(addr.Network->IfIdx, router.InterfaceIndex);
            Assert.True(addr.Flags.HasFlag(WinDivertAddressFlag.Outbound));
            Assert.True(addr.Flags.HasFlag(WinDivertAddressFlag.Loopback));
        }

        [Fact]
        public void IPv6LoopbackTest()
        {
            var router = new WinDivertRouter(IPAddress.IPv6Loopback);
            Assert.Equal(IPAddress.IPv6Loopback, router.SrcAddress);
            Assert.Equal(IPAddress.IPv6Loopback, router.DstAddress);
            Assert.True(router.IsOutbound);
            Assert.True(router.IsLoopback);
        }

        [Fact]
        public void IPv41111Test()
        {
            var dstAddrr = IPAddress.Parse("1.1.1.1");
            var router = new WinDivertRouter(dstAddrr);
            Assert.NotEqual(IPAddress.Any, router.SrcAddress);
            Assert.Equal(dstAddrr, router.DstAddress);
            Assert.True(router.IsOutbound);
            Assert.False(router.IsLoopback);
        }

        [Fact]
        public unsafe void IPv41111AddrTest()
        {
            var dstAddrr = IPAddress.Parse("1.1.1.1");
            var router = new WinDivertRouter(dstAddrr);

            using var addr = new WinDivertAddress
            {
                Flags = WinDivertAddressFlag.Loopback
            };
            router.ApplyToAddress(addr);

            Assert.Equal(addr.Network->IfIdx, router.InterfaceIndex);
            Assert.True(addr.Flags.HasFlag(WinDivertAddressFlag.Outbound));
            Assert.False(addr.Flags.HasFlag(WinDivertAddressFlag.Loopback));
        }
    }
}
