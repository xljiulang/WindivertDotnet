using System;
using System.Net;

namespace WindivertDotnet
{
    /// <summary>
    /// Filter
    /// </summary>
    public interface IFilter
    {
        [FilterName("outbound")]
        bool Outbound { get; }

        [FilterName("inbound")]
        bool Inbound { get; }

        [FilterName("ip")]
        IIP Ip { get; }

        [FilterName("tcp")]
        bool TcpProtocol { get; }

        [FilterName("tcp")]
        ITcp Tcp { get; }

        [FilterName("udp")]
        bool UdpProtocol { get; }

        [FilterName("udp")]
        IUdp Udp { get; }

        [FilterName("ifIdx")]
        int IfIdx { get; }

        [FilterName("subIfIdx")]
        int SubIfIdx { get; }

        public interface IIP
        {
            int Checksum { get; }
            int DF { get; }
            IPAddress DstAddr { get; }
            int FragOff { get; }
            int HdrLength { get; }
            int Id { get; }
            int Length { get; }
            int MF { get; }
            int Protocol { get; }
            IPAddress SrcAddr { get; }
            int TOS { get; }
            int TTL { get; }
        }


        public interface ITransfer
        {
            int Checksum { get; }
            int PayloadLength { get; }
            int SrcPort { get; }
            int DstPort { get; }
        }

        public interface ITcp : ITransfer
        {
            int SeqNum { get; }
            bool Syn { get; }
            bool Rst { get; }
            bool Ack { get; }
        }

        public interface IUdp : ITransfer
        {
            int Length { get; }
        }


        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public sealed class FilterNameAttribute : Attribute
        {
            public string Name { get; }

            public FilterNameAttribute(string name)
            {
                this.Name = name;
            }
        }
    }
}
