using System;

namespace WindivertDotnet
{
    /// <summary>
    /// Filter
    /// </summary>
    public interface IFilter
    {
        [FilterMember("outbound")]
        bool Outbound { get; }

        [FilterMember("inbound")]
        bool Inbound { get; }

        [FilterMember("ip")]
        IIP Ip { get; }

        [FilterMember("tcp")]
        bool IsTcp { get; }

        [FilterMember("tcp")]
        ITcp Tcp { get; }

        [FilterMember("udp")]
        bool IsUdp { get; }

        [FilterMember("udp")]
        IUdp Udp { get; }

        [FilterMember("ifIdx")]
        int IfIdx { get; }

        [FilterMember("subIfIdx")]
        int SubIfIdx { get; }

        public interface IIP
        {

            [FilterMember]
            int Checksum { get; }

            [FilterMember]
            int DF { get; }

            [FilterMember]
            string DstAddr { get; }

            [FilterMember]
            int FragOff { get; }

            [FilterMember]
            int HdrLength { get; }

            [FilterMember]
            int Id { get; }

            [FilterMember]
            int Length { get; }

            [FilterMember]
            int MF { get; }

            [FilterMember]
            int Protocol { get; }

            [FilterMember]
            string SrcAddr { get; }

            [FilterMember]
            int TOS { get; }

            [FilterMember]
            int TTL { get; }
        }


        public interface ITransfer
        {

            [FilterMember]
            int Checksum { get; }

            [FilterMember]
            int PayloadLength { get; }

            [FilterMember]
            int SrcPort { get; }

            [FilterMember]
            int DstPort { get; }
        }

        public interface ITcp : ITransfer
        {
            [FilterMember]
            int SeqNum { get; }

            [FilterMember]
            bool Syn { get; }

            [FilterMember]
            bool Rst { get; }

            [FilterMember]
            bool Ack { get; }
        }

        public interface IUdp : ITransfer
        {
            [FilterMember]
            int Length { get; }
        }


        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public sealed class FilterMemberAttribute : Attribute
        {
            public string? Name { get; }

            public FilterMemberAttribute()
            {
            }

            public FilterMemberAttribute(string name)
            {
                this.Name = name;
            }
        }
    }
}
