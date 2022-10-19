using System;
using System.Buffers.Binary;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace WindivertDotnet.Test
{
    public class WinDivertBufferWriterTest
    {
        [Fact]
        public void WriteByteTest()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.Write(byte.MaxValue);
            Assert.Equal(1, packet.Length);
            Assert.Equal(byte.MaxValue, packet.Span[0]);
        }


        [Fact]
        public void WriteSpanTest()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.Write(new byte[] { 1, 2 });
            Assert.Equal(2, packet.Length);
            Assert.True(packet.Span.SequenceEqual(new byte[] { 1, 2 }));
        }


        [Fact]
        public void WriteInt32Test()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.Write(100);
            Assert.Equal(sizeof(int), packet.Length);
            Assert.Equal(100, BitConverter.ToInt32(packet.Span));
        }

        [Fact]
        public void WriteMyStructTest()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            var myStruct = new MyStruct { Byte1 = 1, Byte2 = 2 };
            writer.Write(myStruct);

            Assert.Equal(Unsafe.SizeOf<MyStruct>(), packet.Length);
            Assert.True(packet.Span[0] == 1 && packet.Span[1] == 2);
        }


        [Fact]
        public void WriteReverseInt32Test()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            writer.WriteReverse(100);
            Assert.Equal(sizeof(int), packet.Length);
            Assert.Equal(BinaryPrimitives.ReverseEndianness(100), BitConverter.ToInt32(packet.Span));
        }

        [Fact]
        public void WriteReverseMyStructTest()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter();
            var myStruct = new MyStruct { Byte1 = 1, Byte2 = 2 };
            writer.WriteReverse(myStruct);

            Assert.Equal(Unsafe.SizeOf<MyStruct>(), packet.Length);
            Assert.True(packet.Span[0] == 2 && packet.Span[1] == 1);
        }

        [Fact]
        public void OffsetLengthTest()
        {
            using var packet = new WinDivertPacket();
            var writer = packet.GetWriter(10);
            writer.Advance(8);

            Assert.Equal(18, packet.Length);
        }

        struct MyStruct
        {
            public byte Byte1;
            public byte Byte2;
        }
    }
}
