using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WindivertDotnet.Test
{
    public unsafe class UdpHeaderTest
    {
        [Fact]
        public void ReadTest()
        {
            var packet = PacketDotNet.UdpPacket.RandomPacket();
            fixed (void* ptr = &packet.Bytes[0])
            {
                var header = *(UdpHeader*)ptr;
                Assert.Equal(packet.SourcePort, header.SrcPort);
                Assert.Equal(packet.Length, header.Length);
                Assert.Equal(packet.Checksum, header.Checksum);
                Assert.Equal(packet.DestinationPort, header.DstPort);
            }
        }

        [Fact]
        public void WriteTest()
        {
            var header = new UdpHeader
            {
                Checksum = 1,
                DstPort = 2,
                SrcPort = 3,
                Length = 4,
            };
            Assert.Equal(1, header.Checksum);
            Assert.Equal(2, header.DstPort);
            Assert.Equal(3, header.SrcPort);
            Assert.Equal(4, header.Length);
        }
    }
}
