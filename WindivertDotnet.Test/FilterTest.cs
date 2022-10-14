using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using Xunit;

namespace WindivertDotnet.Test
{
    [SupportedOSPlatform("windows")]
    public class FilterTest
    {
        [Fact]
        public void TrueTest()
        {
            var filter = Filter.True.ToString();
            Assert.Equal("true", filter);
        }

        [Fact]
        public void FlaseTest()
        {
            var filter = Filter.False.ToString();
            Assert.Equal("false", filter);
        }

        [Fact]
        public void AndExpressionTest()
        {
            var filter = Filter.True.And(f => false).ToString();
            Assert.Equal("false", filter);
        }

        [Fact]
        public void OrExpressionTest()
        {
            var filter = Filter.True.Or(f => false).ToString();
            Assert.Equal("true", filter);
        }

        [Fact]
        public void AndFilterTest()
        {
            var filter = Filter.True.And(Filter.False).ToString();
            Assert.Equal("false", filter);
        }

        [Fact]
        public void OrFilterTest()
        {
            var filter = Filter.True.Or(Filter.False).ToString();
            Assert.Equal("true", filter);
        }

        [Fact]
        public void BaseTest()
        {
            var filter = Filter.False
                .Or(f => f.IsIcmp)
                .Or(f => f.IsIcmpV6)
                .Or(f => f.IsIP)
                .Or(f => f.IsIPV6)
                .Or(f => f.IsTcp)
                .Or(f => f.IsUdp)
                .Or(f => f.Timestamp > 1)
                ;

            using var divert1 = new WinDivert(filter, WinDivertLayer.Network);
            using var divert2 = new WinDivert(filter, WinDivertLayer.Forward);
            using var divert3 = new WinDivert(filter, WinDivertLayer.Socket, 0, WinDivertFlag.ReadOnly);
        }


        [Fact]
        public void NetworkTest()
        {
            var filter = Filter.False
                .Or(f => f.Network.Fragment)
                .Or(f => f.Network.Inbound)
                .Or(f => f.Network.Length > 0)
                .Or(f => f.Network.IfIdx > 0)
                .Or(f => f.Network.Impostor)
                .Or(f => f.Network.LocalAddr == IPAddress.Loopback.ToString())
                .Or(f => f.Network.LocalPort > 0)
                .Or(f => f.Network.Loopback)
                .Or(f => f.Network.Outbound)
                .Or(f => f.Network.Protocol == (int)ProtocolType.Udp)
                .Or(f => f.Network.RemoteAddr == IPAddress.Loopback.ToString())
                .Or(f => f.Network.RemotePort == 443)
                .Or(f => f.Network.SubIfIdx == 12);

            using var divert = new WinDivert(filter, WinDivertLayer.Network);
        }
    }
}