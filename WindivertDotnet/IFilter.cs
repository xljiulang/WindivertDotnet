using System;

namespace WindivertDotnet
{
    /// <summary>
    /// 定义filter的属性
    /// </summary>
    public interface IFilter
    {
        [FilterMember("timestamp")]
        long Timestamp { get; }

        [FilterMember("event")]
        string Event { get; set; }

        INetwork Network { get; }

        IForward Forward { get; }

        ISocket Socket { get; }

        IReflect Reflect { get; }

        /// <summary>
        /// 获取ip对象
        /// </summary>
        [FilterMember("ip")]
        IIP Ip { get; }

        [FilterMember("ip")]
        bool IsIP { get; }

        [FilterMember("ipv6")]
        IIPV6 IPV6 { get; }

        [FilterMember("ipv6")]
        bool IsIPV6 { get; }

        /// <summary>
        /// 获取udp对象
        /// </summary>
        [FilterMember("tcp")]
        ITcp Tcp { get; }

        /// <summary>
        /// 是否为tcp协议
        /// </summary>
        [FilterMember("tcp")]
        bool IsTcp { get; }


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

        [FilterMember("icmp")]
        bool IsIcmp { get; }

        [FilterMember("icmp")]
        public IICmp ICmp { get; }

        [FilterMember("icmpv6")]
        bool IsIcmpV6 { get; }

        [FilterMember("icmpv6")]
        public IICmp IcmpV6 { get; }

        public interface INetwork
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

            [FilterMember("loopback")]
            bool Loopback { get; }


            [FilterMember("impostor")]
            bool Impostor { get; }


            [FilterMember("fragment")]
            bool Fragment { get; }

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

            [FilterMember("protocol")]
            int Protocol { get; }

            [FilterMember("localAddr")]
            string LocalAddr { get; }

            [FilterMember("localPort")]
            int LocalPort { get; }

            [FilterMember("remoteAddr")]
            string RemoteAddr { get; }

            [FilterMember("remotePort")]
            int RemotePort { get; }

            /// <summary>
            /// The packet length
            /// </summary>
            [FilterMember("length")]
            int Length { get; }
        }

        public interface IForward
        {
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


            [FilterMember("impostor")]
            bool Impostor { get; }


            [FilterMember("fragment")]
            bool Fragment { get; }

            /// <summary>
            /// The packet length
            /// </summary>
            [FilterMember("length")]
            int Length { get; }
        }

        public interface IFlow
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

            [FilterMember("loopback")]
            bool Loopback { get; }

            [FilterMember("endpointId")]
            int EndpointId { get; }

            [FilterMember("parentEndpointId")]
            int ParentEndpointId { get; }

            [FilterMember("processId")]
            int ProcessId { get; }

            [FilterMember("localAddr")]
            string LocalAddr { get; }

            [FilterMember("localPort")]
            int LocalPort { get; }

            [FilterMember("remoteAddr")]
            string RemoteAddr { get; }

            [FilterMember("remotePort")]
            int RemotePort { get; }

            [FilterMember("protocol")]
            int Protocol { get; }
        }

        public interface ISocket
        {
            [FilterMember("loopback")]
            bool Loopback { get; }

            [FilterMember("endpointId")]
            int EndpointId { get; }

            [FilterMember("parentEndpointId")]
            int ParentEndpointId { get; }

            [FilterMember("processId")]
            int ProcessId { get; }

            [FilterMember("localAddr")]
            string LocalAddr { get; }

            [FilterMember("localPort")]
            int LocalPort { get; }

            [FilterMember("remoteAddr")]
            string RemoteAddr { get; }

            [FilterMember("remotePort")]
            int RemotePort { get; }

            [FilterMember("protocol")]
            int Protocol { get; }
        }

        public interface IReflect
        {
            [FilterMember("processId")]
            int ProcessId { get; }

            [FilterMember("layer")]
            string Layer { get; }

            [FilterMember("priority")]
            int Priority { get; }
        }

        /// <summary>
        /// IP对象
        /// </summary>
        public interface IIP
        {
            [FilterMember]
            int HdrLength { get; }

            [FilterMember]
            int Version { get; }

            [FilterMember]
            int TOS { get; }

            [FilterMember]
            int Length { get; }

            [FilterMember]
            int Id { get; }

            [FilterMember]
            int TTL { get; }

            [FilterMember]
            int Protocol { get; }

            [FilterMember]
            int Checksum { get; }

            [FilterMember]
            string SrcAddr { get; }

            [FilterMember]
            string DstAddr { get; }
        }

        public interface IIPV6
        {
            [FilterMember]
            int Version { get; }

            [FilterMember]
            int Length { get; }

            [FilterMember]
            int NextHdr { get; }

            [FilterMember]
            int HopLimit { get; }

            [FilterMember]
            string SrcAddr { get; }

            [FilterMember]
            string DstAddr { get; }
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
            int AckNum { get; }

            [FilterMember]
            bool Fin { get; }

            [FilterMember]
            bool Syn { get; }

            [FilterMember]
            bool Rst { get; }

            [FilterMember]
            bool Ack { get; }

            [FilterMember]
            bool Psh { get; }

            [FilterMember]
            bool Urg { get; }

            [FilterMember]
            int Window { get; }

            [FilterMember]
            int UrgPtr { get; }
        }

        public interface IUdp : ITransfer
        {
            [FilterMember]
            int Length { get; }
        }

        public interface IICmp
        {
            [FilterMember]
            int Type { get; }

            [FilterMember]
            int Code { get; }

            [FilterMember]
            int Checksum { get; }

            [FilterMember]
            int Body { get; }
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
