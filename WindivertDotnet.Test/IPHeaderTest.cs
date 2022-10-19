using System.Net;
using System.Net.Sockets;
using Xunit;

namespace WindivertDotnet.Test
{
    public class IPHeaderTest
    {
        [Fact]
        public unsafe void IPV4ReadTest()
        {
            var packet = PacketDotNet.IPPacket.RandomPacket(PacketDotNet.IPVersion.IPv4);
            fixed (void* ptr = &packet.Bytes[0])
            {
                var header = *(IPV4Header*)ptr;
                Assert.Equal((byte)packet.Version, header.Version);
                Assert.Equal(packet.HeaderLength, header.HdrLength);
                Assert.Equal(packet.TotalLength, header.Length);
                Assert.Equal(packet.TimeToLive, header.TTL);
                Assert.Equal((byte)packet.Protocol, (byte)header.Protocol);
                Assert.Equal(packet.SourceAddress, header.SrcAddr);
                Assert.Equal(packet.DestinationAddress, header.DstAddr);
            }
        }

        [Fact]
        public unsafe void IPV4WriteTest()
        {
            var dstAddr = IPAddress.Parse("1.2.3.4");
            var srcAddr = IPAddress.Parse("5.6.7.8");

            var header = new IPV4Header
            {
                Version = 1,
                Checksum = 2,
                DstAddr = dstAddr,
                SrcAddr = srcAddr,
                TOS = 3,
                FragOff0 = 4,
                HdrLength = 5,
                Id = 6,
                Length = 7,
                Protocol = ProtocolType.Ggp,
                TTL = 8
            };


            Assert.Equal(1, header.Version);
            Assert.Equal(2, header.Checksum);

            Assert.Equal(dstAddr, header.DstAddr);
            Assert.Equal(srcAddr, header.SrcAddr);
            Assert.Equal(3, header.TOS);
            Assert.Equal(4, header.FragOff0);
            Assert.Equal(5, header.HdrLength);
            Assert.Equal(6, header.Id);

            Assert.Equal(7, header.Length);
            Assert.Equal(ProtocolType.Ggp, header.Protocol);
            Assert.Equal(8, header.TTL);
        }

        [Fact]
        public unsafe void IPV6ReadTest()
        {
            var packet = PacketDotNet.IPPacket.RandomPacket(PacketDotNet.IPVersion.IPv6);
            fixed (void* ptr = &packet.Bytes[0])
            {
                var header = *(IPV6Header*)ptr;
                Assert.Equal((byte)packet.Version, header.Version);
                Assert.Equal(packet.PayloadLength, header.Length);
                Assert.Equal(packet.HopLimit, header.HopLimit);
                Assert.Equal((byte)packet.Protocol, (byte)header.NextHdr);
                Assert.Equal(packet.SourceAddress, header.SrcAddr);
                Assert.Equal(packet.DestinationAddress, header.DstAddr);
            }
        }

        [Fact]
        public unsafe void IPV6WriteTest()
        {
            var dstAddr = IPAddress.IPv6Loopback;
            var srcAddr = IPAddress.IPv6Loopback;

            var header = new IPV6Header
            {
                Version = 1,
                FlowLabel0 = 2,
                FlowLabel1 = 3,
                HopLimit = 4,
                Length = 5,
                TrafficClass0 = 6,
                TrafficClass1 = 7,
                DstAddr = dstAddr,
                SrcAddr = srcAddr,
                NextHdr = ProtocolType.Ggp,

            };

            Assert.Equal(1, header.Version);
            Assert.Equal(2, header.FlowLabel0);

            Assert.Equal(dstAddr, header.DstAddr);
            Assert.Equal(srcAddr, header.SrcAddr);
            Assert.Equal(3, header.FlowLabel1);
            Assert.Equal(4, header.HopLimit);
            Assert.Equal(5, header.Length);
            Assert.Equal(6, header.TrafficClass0);

            Assert.Equal(7, header.TrafficClass1);
            Assert.Equal(ProtocolType.Ggp, header.NextHdr);
        }
    }
}
