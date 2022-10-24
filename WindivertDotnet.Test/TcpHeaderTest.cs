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
                Assert.Equal(packet.Finished, header.Fin);
                Assert.Equal(packet.Checksum, header.Checksum);
                Assert.Equal(packet.DestinationPort, header.DstPort);
                Assert.Equal(packet.Synchronize, header.Syn);
                Assert.Equal(packet.SequenceNumber, header.SeqNum);
                Assert.Equal(packet.WindowSize, header.Window);
                Assert.Equal(packet.Acknowledgment, header.Ack);
                Assert.Equal(packet.AcknowledgmentNumber, header.AckNum);
                Assert.Equal(packet.Reset, header.Rst);
                Assert.Equal(packet.Push, header.Psh);
                Assert.Equal(packet.Urgent, header.Urg);
                Assert.Equal(packet.UrgentPointer, header.UrgPtr);
            }
        }

        [Fact]
        public void WriteTest()
        {
            var header = new TcpHeader
            {
                Ack = true,
                Fin = true,
                Psh = true,
                Syn = true,
                Urg = true,
                Rst = true,



                AckNum = 2,
                Checksum = 3,
                DstPort = 4,
                HdrLength = 6,
                UrgPtr = 8,

                SeqNum = 12,
                SrcPort = 13,
                Window = 16
            };

            Assert.True(header.Flags.HasFlag(TcpFlag.Ack));
            Assert.True(header.Flags.HasFlag(TcpFlag.Fin));
            Assert.True(header.Flags.HasFlag(TcpFlag.Psh));
            Assert.True(header.Flags.HasFlag(TcpFlag.Rst));
            Assert.True(header.Flags.HasFlag(TcpFlag.Fin));
            Assert.True(header.Flags.HasFlag(TcpFlag.Syn));

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
