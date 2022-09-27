using System.Net;

namespace WindivertDotnet
{
    /// <summary>
    /// Filter
    /// </summary>
    public interface IFilter
    {
        bool Outbound { get; }

        bool Inbound { get; }

        IIP Ip { get; }

        bool IsTcp { get; }

        ITcp Tcp { get; }

        bool IsUdp { get; }

        IUdp Udp { get; }

        int IfIdx { get; }

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
    }
}
