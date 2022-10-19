using System.Runtime.CompilerServices;
using Xunit;

namespace WindivertDotnet.Test
{
    public  class HeaderSizeTest
    {
        [Fact]
        public void IPV4HeaderSizeTest()
        { 
            Assert.Equal(20, Unsafe.SizeOf<IPV4Header>());
        }

        [Fact]
        public void IPV6HeaderSizeTest()
        {
            Assert.Equal(40, Unsafe.SizeOf<IPV6Header>());
        }

        [Fact]
        public void IcmpV4HeaderSizeTest()
        {
            Assert.Equal(8, Unsafe.SizeOf<IcmpV4Header>());
        }

        [Fact]
        public void IcmpV6HeaderSizeTest()
        {
            Assert.Equal(8, Unsafe.SizeOf<IcmpV6Header>());
        }

        [Fact]
        public void TcpHeaderHeaderSizeTest()
        {
            Assert.Equal(20, Unsafe.SizeOf<TcpHeader>());
        }

        [Fact]
        public void UdpHeaderHeaderSizeTest()
        {
            Assert.Equal(8, Unsafe.SizeOf<UdpHeader>());
        }
    }
}
