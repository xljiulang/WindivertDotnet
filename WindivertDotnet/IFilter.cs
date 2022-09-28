using System;

namespace WindivertDotnet
{
    /// <summary>
    /// 定义filter的属性
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        [FilterMember("timestamp")]
        long Timestamp { get; }

        /// <summary>
        /// 事件类型
        /// </summary>
        [FilterMember("event")]
        string Event { get; set; }

        /// <summary>
        /// Network层
        /// </summary>
        INetwork Network { get; }

        /// <summary>
        /// Forward层
        /// </summary>
        IForward Forward { get; }

        /// <summary>
        /// Socket层
        /// </summary>
        ISocket Socket { get; }

        /// <summary>
        /// Reflect层
        /// </summary>
        IReflect Reflect { get; }

        /// <summary>
        /// 是否为IPv4
        /// </summary>
        [FilterMember("ip")]
        bool IsIP { get; }

        /// <summary>
        /// IPv4的属性集
        /// </summary>
        [FilterMember("ip")]
        IIP IP { get; }

        /// <summary>
        /// 是否IPv6
        /// </summary>
        [FilterMember("ipv6")]
        bool IsIPV6 { get; }

        /// <summary>
        /// IPv6属性集
        /// </summary>
        [FilterMember("ipv6")]
        IIPV6 IPV6 { get; }

        /// <summary>
        /// 是否为tcp协议
        /// </summary>
        [FilterMember("tcp")]
        bool IsTcp { get; }

        /// <summary>
        /// tcp属性集
        /// </summary>
        [FilterMember("tcp")]
        ITcp Tcp { get; }


        /// <summary>
        /// 是否udp协议
        /// </summary>
        [FilterMember("udp")]
        bool IsUdp { get; }

        /// <summary>
        /// udp属性集
        /// </summary>
        [FilterMember("udp")]
        IUdp Udp { get; }


        /// <summary>
        /// 是否为icmpV4协议
        /// </summary>
        [FilterMember("icmp")]
        bool IsIcmp { get; }

        /// <summary>
        /// icmpV4属性集
        /// </summary>
        [FilterMember("icmp")]
        public IICmp ICmp { get; }

        /// <summary>
        /// 是否为icmpV6协议
        /// </summary>
        [FilterMember("icmpv6")]
        bool IsIcmpV6 { get; }

        /// <summary>
        /// icmpV6属性集
        /// </summary>
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

            /// <summary>
            /// 回环IP
            /// </summary>
            [FilterMember("loopback")]
            bool Loopback { get; }

            /// <summary>
            /// 是否已修改
            /// </summary>
            [FilterMember("impostor")]
            bool Impostor { get; }

            /// <summary>
            /// 是否分片
            /// </summary>
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

            /// <summary>
            /// 协议类型
            /// <para>ProtocolType</para>
            /// </summary>
            [FilterMember("protocol")]
            int Protocol { get; }

            /// <summary>
            /// 本机IP地址
            /// </summary>
            [FilterMember("localAddr")]
            string LocalAddr { get; }

            /// <summary>
            /// 本机端口
            /// </summary>
            [FilterMember("localPort")]
            int LocalPort { get; }

            /// <summary>
            /// 远程IP地址
            /// </summary>
            [FilterMember("remoteAddr")]
            string RemoteAddr { get; }

            /// <summary>
            /// 远程端口
            /// </summary>
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
        /// IPV4对象
        /// </summary>
        public interface IIP
        {
            [FilterMember]
            int HdrLength { get; }

            /// <summary>
            /// 版本
            /// </summary>
            [FilterMember]
            int Version { get; }

            [FilterMember]
            int TOS { get; }

            /// <summary>
            /// 包长度
            /// </summary>
            [FilterMember]
            int Length { get; }

            [FilterMember]
            int Id { get; }

            [FilterMember]
            int TTL { get; }

            /// <summary>
            /// 协议
            /// <para>ProtocolType</para>
            /// </summary>
            [FilterMember]
            int Protocol { get; }

            /// <summary>
            /// 检验和
            /// </summary>
            [FilterMember]
            int Checksum { get; }

            /// <summary>
            /// 源IP地址
            /// </summary>
            [FilterMember]
            string SrcAddr { get; }

            /// <summary>
            /// 远程IP地址
            /// </summary>
            [FilterMember]
            string DstAddr { get; }
        }

        public interface IIPV6
        {
            /// <summary>
            /// 版本
            /// </summary>
            [FilterMember]
            int Version { get; }

            /// <summary>
            /// 有效负载长度
            /// </summary>
            [FilterMember]
            int Length { get; }

            [FilterMember]
            int NextHdr { get; }

            [FilterMember]
            int HopLimit { get; }

            /// <summary>
            /// 源IP地址
            /// </summary>
            [FilterMember]
            string SrcAddr { get; }

            /// <summary>
            /// 目的IP地址
            /// </summary>
            [FilterMember]
            string DstAddr { get; }
        }


        public interface ITransfer
        {
            /// <summary>
            /// 校验和
            /// </summary>
            [FilterMember]
            int Checksum { get; }

            /// <summary>
            /// 负载数据长度
            /// </summary>
            [FilterMember]
            int PayloadLength { get; }

            /// <summary>
            /// 源端口号
            /// </summary>
            [FilterMember]
            int SrcPort { get; }

            /// <summary>
            /// 目的端口号
            /// </summary>
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
            /// <summary>
            /// 获取或设置Udp包长度
            /// <para>含头部的8字节</para>
            /// </summary>
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
