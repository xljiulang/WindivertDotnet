using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using Xunit;

namespace WindivertDotnet.Test
{
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
        [SupportedOSPlatform("windows")]
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

            var f1 = Filter.Compile(filter.ToString(), WinDivertLayer.Network);
            var f2 = Filter.Compile(filter.ToString(), WinDivertLayer.Forward);
            var f3 = Filter.Compile(filter.ToString(), WinDivertLayer.Socket);

            Assert.True(f1.Length > 0);
            Assert.True(f2.Length > 0);
            Assert.True(f3.Length > 0);
        }


        [Fact]
        [SupportedOSPlatform("windows")]
        public void NetworkTest()
        {
            var filter = Filter.False
                .Or(f => f.Event == Event.PACKET)
                .Or(f => f.Network.Fragment)
                .Or(f => f.Network.Inbound)
                .Or(f => f.Network.Length > 0)
                .Or(f => f.Network.IfIdx > 0)
                .Or(f => f.Network.Impostor)
                .Or(f => f.Network.LocalAddr == IPAddress.Loopback.ToString())
                .Or(f => f.Network.LocalPort > 0)
                .Or(f => f.Network.Loopback)
                .Or(f => f.Network.Outbound)
                .Or(f => f.Network.Protocol == ProtocolType.Udp)
                .Or(f => f.Network.RemoteAddr == IPAddress.Loopback.ToString())
                .Or(f => f.Network.RemotePort == 443)
                .Or(f => f.Network.SubIfIdx == 12);

            var f = Filter.Compile(filter.ToString(), WinDivertLayer.Network);
            Assert.True(f.Length > 0);
        }
    }
}