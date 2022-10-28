using System;
using System.Net.Sockets;

namespace WindivertDotnet
{
    /// <summary>
    /// 定义filter的属性
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 发生的时间戳
        /// </summary>
        [FilterMember("timestamp")]
        long Timestamp { get; }

        /// <summary>
        /// 事件
        /// </summary>
        [FilterMember("event")]
        Event Event { get; }

        /// <summary>
        /// 传入/传出本地计算机的网络数据包
        /// </summary>
        INetwork Network { get; }

        /// <summary>
        /// 通过本地计算机的网络数据包
        /// </summary>
        IForward Forward { get; }

        /// <summary>
        /// 网络流已建立/已删除事件
        /// </summary>
        IFlow Flow { get; }

        /// <summary>
        /// 套接字操作事件
        /// </summary>
        ISocket Socket { get; }

        /// <summary>
        /// WinDivert处理事件
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
        public IICmpV6 IcmpV6 { get; }


        /// <summary>
        /// 传入/传出本地计算机的网络数据包
        /// </summary>
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
            ProtocolType Protocol { get; }

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

        /// <summary>
        /// 通过本地计算机的网络数据包
        /// </summary>
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

            /// <summary>
            /// 冒名顶替数据包
            /// </summary>
            [FilterMember("impostor")]
            bool Impostor { get; }

            /// <summary>
            /// ip分片包
            /// </summary>
            [FilterMember("fragment")]
            bool Fragment { get; }

            /// <summary>
            /// The packet length
            /// </summary>
            [FilterMember("length")]
            int Length { get; }
        }

        /// <summary>
        /// 网络流已建立/已删除事件
        /// </summary>
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

            /// <summary>
            /// 环回数据包
            /// </summary>
            [FilterMember("loopback")]
            bool Loopback { get; }

            /// <summary>
            /// 流的终结点ID
            /// </summary>
            [FilterMember("endpointId")]
            int EndpointId { get; }

            /// <summary>
            /// 流的父终结点ID
            /// </summary>
            [FilterMember("parentEndpointId")]
            int ParentEndpointId { get; }

            /// <summary>
            /// 与流关联的进程的ID
            /// </summary>
            [FilterMember("processId")]
            int ProcessId { get; }

            /// <summary>
            /// 本机地址
            /// </summary>
            [FilterMember("localAddr")]
            string LocalAddr { get; }

            /// <summary>
            /// 本机端口
            /// </summary>
            [FilterMember("localPort")]
            int LocalPort { get; }

            /// <summary>
            /// 远程地址
            /// </summary>
            [FilterMember("remoteAddr")]
            string RemoteAddr { get; }

            /// <summary>
            /// 远程端口
            /// </summary>
            [FilterMember("remotePort")]
            int RemotePort { get; }

            /// <summary>
            /// 协议类型(ProtocolType)
            /// </summary>
            [FilterMember("protocol")]
            ProtocolType Protocol { get; }
        }

        /// <summary>
        /// 套接字操作事件
        /// </summary>
        public interface ISocket
        {
            /// <summary>
            /// 环回数据包
            /// </summary>
            [FilterMember("loopback")]
            bool Loopback { get; }

            /// <summary>
            /// 套接字操作的终结点 ID
            /// </summary>
            [FilterMember("endpointId")]
            int EndpointId { get; }

            /// <summary>
            /// 套接字操作的父终结点 ID
            /// </summary>
            [FilterMember("parentEndpointId")]
            int ParentEndpointId { get; }

            /// <summary>
            /// 与套接字操作关联的进程的 ID
            /// </summary>
            [FilterMember("processId")]
            int ProcessId { get; }

            /// <summary>
            /// 本机地址
            /// </summary>
            [FilterMember("localAddr")]
            string LocalAddr { get; }

            /// <summary>
            /// 本机端口
            /// </summary>
            [FilterMember("localPort")]
            int LocalPort { get; }

            /// <summary>
            /// 远程地址
            /// </summary>
            [FilterMember("remoteAddr")]
            string RemoteAddr { get; }

            /// <summary>
            /// 远程端口
            /// </summary>
            [FilterMember("remotePort")]
            int RemotePort { get; }

            /// <summary>
            /// 协议类型(ProtocolType)
            /// </summary>
            [FilterMember("protocol")]
            ProtocolType Protocol { get; }
        }

        /// <summary>
        /// WinDivert处理事件
        /// </summary>
        public interface IReflect
        {
            /// <summary>
            /// WinDivert打开句柄的进程的ID
            /// </summary>
            [FilterMember("processId")]
            int ProcessId { get; }

            /// <summary>
            /// WinDivertLayer
            /// </summary>
            [FilterMember("layer")]
            string Layer { get; }
            /// <summary>
            /// 优先级
            /// </summary>
            [FilterMember("priority")]
            int Priority { get; }
        }

        /// <summary>
        /// IPV4对象
        /// </summary>
        public interface IIP
        {
            /// <summary>
            /// 获取或设置Internet Header Length
            /// ipv4头总字节为该值的4倍
            /// </summary>
            [FilterMember]
            int HdrLength { get; }

            /// <summary>
            /// 版本
            /// </summary>
            [FilterMember]
            int Version { get; }

            /// <summary>
            /// 服务类型
            /// </summary>
            [FilterMember]
            int TOS { get; }

            /// <summary>
            /// 包长度
            /// </summary>
            [FilterMember]
            int Length { get; }

            /// <summary>
            /// 报文的id
            /// </summary>
            [FilterMember]
            int Id { get; }

            /// <summary>
            /// 生存时间
            /// </summary>
            [FilterMember]
            int TTL { get; }

            /// <summary>
            /// 协议
            /// <para>ProtocolType</para>
            /// </summary>
            [FilterMember]
            ProtocolType Protocol { get; }

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

        /// <summary>
        /// ipv6
        /// </summary>
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

            /// <summary>
            /// 下一个报头
            /// </summary>
            [FilterMember]
            int NextHdr { get; }

            /// <summary>
            /// 跳跃限制
            /// </summary>
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

        /// <summary>
        /// 传输层
        /// </summary>
        public interface ITransfer
        {
            /// <summary>
            /// 校验和
            /// </summary>
            [FilterMember]
            int Checksum { get; }

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

            /// <summary>
            /// 负载数据长度
            /// </summary>
            [FilterMember]
            int PayloadLength { get; }

            /// <summary>
            /// 8位形式的负载数据
            /// 使用索引来读取
            /// </summary>
            [FilterMember]
            byte[] Payload { get; }

            /// <summary>
            /// 16位形式的负载数据
            /// 使用索引来读取
            /// </summary>
            [FilterMember]
            ushort[] Payload16 { get; }

            /// <summary>
            /// 32位形式的负载数据
            /// 使用索引来读取
            /// </summary>
            [FilterMember]
            uint[] Payload32 { get; }
        }

        /// <summary>
        /// tcp
        /// </summary>
        public interface ITcp : ITransfer
        {
            /// <summary>
            /// 序列码
            /// </summary>
            [FilterMember]
            int SeqNum { get; }

            /// <summary>
            /// 确认码
            /// </summary>
            [FilterMember]
            int AckNum { get; }

            /// <summary>
            /// 结束位
            /// </summary>
            [FilterMember]
            bool Fin { get; }

            /// <summary>
            /// 请求位
            /// </summary>
            [FilterMember]
            bool Syn { get; }

            /// <summary>
            /// 重置位
            /// </summary>
            [FilterMember]
            bool Rst { get; }

            /// <summary>
            /// 确认位
            /// </summary>
            [FilterMember]
            bool Ack { get; }

            /// <summary>
            /// 推送位
            /// </summary>
            [FilterMember]
            bool Psh { get; }

            /// <summary>
            /// 紧急位
            /// </summary>
            [FilterMember]
            bool Urg { get; }

            /// <summary>
            /// 滑动窗口
            /// </summary>
            [FilterMember]
            int Window { get; }

            /// <summary>
            /// 紧急指针
            /// </summary>
            [FilterMember]
            int UrgPtr { get; }
        }

        /// <summary>
        /// udp
        /// </summary>
        public interface IUdp : ITransfer
        {
            /// <summary>
            /// 获取或设置Udp包长度
            /// <para>含头部的8字节</para>
            /// </summary>
            [FilterMember]
            int Length { get; }
        }

        /// <summary>
        /// ICMP
        /// </summary>
        public interface IICmp
        {
            /// <summary>
            /// 类型
            /// </summary>
            [FilterMember]
            IcmpV4MessageType Type { get; }

            /// <summary>
            /// 代码
            /// </summary>
            [FilterMember]
            IcmpV4UnreachableCode Code { get; }

            /// <summary>
            /// 检验和
            /// </summary>
            [FilterMember]
            int Checksum { get; }

            /// <summary>
            /// Rest of header
            /// </summary>
            [FilterMember]
            int Body { get; }
        }

        /// <summary>
        /// ICMP
        /// </summary>
        public interface IICmpV6
        {
            /// <summary>
            /// 类型
            /// </summary>
            [FilterMember]
            IcmpV6MessageType Type { get; }

            /// <summary>
            /// 代码
            /// </summary>
            [FilterMember]
            IcmpV6UnreachableCode Code { get; }

            /// <summary>
            /// 检验和
            /// </summary>
            [FilterMember]
            int Checksum { get; }

            /// <summary>
            /// Rest of header
            /// </summary>
            [FilterMember]
            int Body { get; }
        }


        /// <summary>
        /// Filter成员特性
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public sealed class FilterMemberAttribute : Attribute
        {
            /// <summary>
            /// 成员名称
            /// </summary>
            public string? Name { get; }

            /// <summary>
            /// Filter成员特性
            /// </summary>
            public FilterMemberAttribute()
            {
            }

            /// <summary>
            /// Filter成员特性
            /// </summary>
            /// <param name="name">成员名称</param>
            public FilterMemberAttribute(string name)
            {
                this.Name = name;
            }
        }
    }
}
