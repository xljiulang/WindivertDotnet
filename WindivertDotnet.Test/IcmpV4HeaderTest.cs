using Xunit;

namespace WindivertDotnet.Test
{
    public unsafe class IcmpV4HeaderTest
    {
        [Fact]
        public void ReadWriteTest()
        {
            var header = new IcmpV4Header
            {
                Checksum = 1,
                Code = IcmpV4UnreachableCode.PrecedenceCutoffInEffect,
                Type = IcmpV4MessageType.TimeExceeded,
                Body = 0x20003
            };

            Assert.Equal(1, header.Checksum);
            Assert.Equal(0x2, header.Identifier);
            Assert.Equal(0x3, header.SequenceNumber);
            Assert.Equal(0x20003, (int)header.Body);
            Assert.Equal(IcmpV4UnreachableCode.PrecedenceCutoffInEffect, header.Code);
            Assert.Equal(IcmpV4MessageType.TimeExceeded, header.Type);
        }
    }
}
