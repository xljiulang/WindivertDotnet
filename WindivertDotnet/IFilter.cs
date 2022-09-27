using System;

namespace WindivertDotnet
{
    /// <summary>
    /// 定义filter的属性
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 是否为出口
        /// </summary>
        [FilterMember("outbound")]
        bool Outbound { get; }

        /// <summary>
        /// 是否为入口
        /// </summary>
        [FilterMember("inbound")]
        bool Inbound { get; }

        /// <summary>
        /// 获取ip对象
        /// </summary>
        [FilterMember("ip")]
        IIP Ip { get; }

        /// <summary>
        /// 是否为tcp协议
        /// </summary>
        [FilterMember("tcp")]
        bool IsTcp { get; }

        /// <summary>
        /// 获取udp对象
        /// </summary>
        [FilterMember("tcp")]
        ITcp Tcp { get; }

        /// <summary>
        /// 是否udp协议
        /// </summary>
        [FilterMember("udp")]
        bool IsUdp { get; }

        /// <summary>
        /// udp对象
        /// </summary>
        [FilterMember("udp")]
        IUdp Udp { get; }

        /// <summary>
        /// 网卡索引
        /// </summary>
        [FilterMember("ifIdx")]
        int IfIdx { get; }

        /// <summary>
        /// 子网卡索引
        /// </summary>
        [FilterMember("subIfIdx")]
        int SubIfIdx { get; }


        /// <summary>
        /// IP对象
        /// </summary>
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
