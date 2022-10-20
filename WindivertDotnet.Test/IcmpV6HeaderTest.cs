using Xunit;

namespace WindivertDotnet.Test
{
    public unsafe class IcmpV6HeaderTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var header = new IcmpV6Header
            {
                Checksum = 1,
                Code = IcmpV6UnreachableCode.PortUnreachable,
                Type = IcmpV6MessageType.EchoRequest,
                Body = 0x20003
            };

            Assert.Equal(1, header.Checksum);
            Assert.Equal(0x2, header.Identifier);
            Assert.Equal(0x3, header.SequenceNumber);
            Assert.Equal((uint)0x20003, header.Body);
            Assert.Equal(IcmpV6UnreachableCode.PortUnreachable, header.Code);
            Assert.Equal(IcmpV6MessageType.EchoRequest, header.Type);
        }
    }
}
