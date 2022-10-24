# WindivertDotnet
面向对象的[WinDivertv2.2](https://github.com/basil00/Divert)的dotnet异步封装，轻松实现网络数据拦截与修改。

### 1 nuget
[WindivertDotnet](https://www.nuget.org/packages/WindivertDotnet)
```
<PackageReference Include="WindivertDotnet" Version="1.*" /> 
```

### 2 功能介绍
* 抓取网络数据包
* 过滤或丢弃网络数据包
* 嗅探网络数据包
* 注入网络数据包
* 修改网络数据包


### 3 如何使用
#### 3.1 抓包改包
```c# 
var filter = Filter.True
    .And(f => f.Network.Loopback)
    .And(f => f.Tcp.DstPort == 443)
    .And(f => f.Tcp.Ack == true);

using var divert = new WinDivert(filter, WinDivertLayer.Network);
using var packet = new WinDivertPacket();
using var addr = new WinDivertAddress();

while (true)
{
    // 读包
    await divert.RecvAsync(packet, addr);

    ProcessPacket(packet, addr); 

    // 修改后发出
    await divert.SendAsync(packet, addr);
}

static unsafe void ProcessPacket(WinDivertPacket packet, WinDivertAddress addr)
{
    // 解包
    var result = packet.GetParseResult();

    // 改包
    result.TcpHeader->DstPort = 443;

    // 重算checksums
    packet.CalcChecksums(addr);
}
```

#### 3.2 注入数据包
```c#
private async Task SendEchoRequestAsync(IPAddress dstAddr)
{
    // 使用router计算将进行通讯的本机地址
    var router = new WinDivertRouter(dstAddr);
    using var addr = router.CreateAddress();
    using var packet = this.CreateIPV4EchoPacket(router.SrcAddress, router.DstAddress);

    packet.CalcChecksums(addr);     // 计算checksums，因为创建包时没有计算

    await this.divert.SendAsync(packet, addr);
}

/// <summary>
/// 创建icmp的echo包
/// </summary>
/// <param name="srcAddr"></param>
/// <param name="dstAddr"></param>
/// <returns></returns>
private unsafe WinDivertPacket CreateIPV4EchoPacket(IPAddress srcAddr, IPAddress dstAddr)
{
    // ipv4头
    var ipHeader = new IPV4Header
    {
        TTL = 128,
        Version =  IPVersion.V4,
        DstAddr = dstAddr,
        SrcAddr = srcAddr,
        Protocol = ProtocolType.Icmp,
        HdrLength = (byte)(sizeof(IPV4Header) / 4),
        Id = ++this.id,
        Length = (ushort)(sizeof(IPV4Header) + sizeof(IcmpV4Header))
    };

    // icmp头
    var icmpHeader = new IcmpV4Header
    {
        Type = IcmpV4MessageType.EchoRequest,
        Code = default,
        Identifier = ipHeader.Id,
        SequenceNumber = ++this.sequenceNumber,
    };

    // 将数据写到packet缓冲区
    var packet = new WinDivertPacket(ipHeader.Length);

    var writer = packet.GetWriter();
    writer.Write(ipHeader);
    writer.Write(icmpHeader);

    return packet;
}
```
