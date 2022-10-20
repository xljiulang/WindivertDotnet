using Xunit;

namespace WindivertDotnet.Test
{
    public unsafe class TcpHeaderTest
    {
        [Fact]
        public void ReadTest()
        {
            var packet = PacketDotNet.TcpPacket.RandomPacket();
            fixed (void* ptr = &packet.Bytes[0])
            {
                var header = *(TcpHeader*)ptr;
                Assert.Equal(packet.SourcePort, header.SrcPort);
                Assert.Equal(packet.Finished, header.Fin == 1);
                Assert.Equal(packet.Checksum, header.Checksum);
                Assert.Equal(packet.DestinationPort, header.DstPort);
                Assert.Equal(packet.Synchronize, header.Syn == 1);
                Assert.Equal(packet.SequenceNumber, header.SeqNum);
                Assert.Equal(packet.WindowSize, header.Window);
                Assert.Equal(packet.Acknowledgment, header.Ack == 1);
                Assert.Equal(packet.AcknowledgmentNumber, header.AckNum);
                Assert.Equal(packet.Reset, header.Rst == 1);
                Assert.Equal(packet.Push, header.Psh == 1);
                Assert.Equal(packet.Urgent, header.Urg == 1);
                Assert.Equal(packet.UrgentPointer, header.UrgPtr);
            }
        }

        [Fact]
        public void WriteTest()
        {
            var header = new TcpHeader
            {
                Ack = 1,
                Fin = 1,
                Psh = 1,
                Syn = 1,
                Urg = 1,
                Rst = 1,
                Reserved1 = 1,
                Reserved2 = 1,

                AckNum = 2,
                Checksum = 3,
                DstPort = 4,
                HdrLength = 6,
                UrgPtr = 8,

                SeqNum = 12,
                SrcPort = 13,
                Window = 16
            };

            Assert.Equal(1, header.Ack);
            Assert.Equal(1, header.Fin);
            Assert.Equal(1, header.Psh);
            Assert.Equal(1, header.Syn = 1);
            Assert.Equal(1, header.Urg = 1);
            Assert.Equal(1, header.Rst = 1);
            Assert.Equal(1, header.Reserved1);
            Assert.Equal(1, header.Reserved2);

            Assert.Equal(2U, header.AckNum);
            Assert.Equal(3, header.Checksum);
            Assert.Equal(4, header.DstPort);
            Assert.Equal(6, header.HdrLength);
            Assert.Equal(8, header.UrgPtr);
           
            Assert.Equal(12U, header.SeqNum);
            Assert.Equal(13, header.SrcPort);
            Assert.Equal(16, header.Window);
        }
    }
}
