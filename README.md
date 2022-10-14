# WindivertDotnet
面向对象的[WinDivertv2.2](https://github.com/basil00/Divert)的dotnet异步封装，轻松实现网络数据拦截与修改。

### 1 nuget
[WindivertDotnet](https://www.nuget.org/packages/WindivertDotnet)

### 2 功能介绍
* Filter对象支持Lambda构建filter language，脱离字符串的苦海；
* 内存安全的WinDivert对象，基于IOCP的ValueTask异步发送与接收方法；
* 内存安全的WinDivertPacket对象，提供获取包有效数据长度、解包、重构chucksums等；
* WinDivertParseResult提供对解包的数据进行精细修改，修改后对WinDivertPacket直接生效；

### 3 如何使用
```
var filter = Filter.True
    .And(f => f.Network.Loopback)
    .And(f => f.Tcp.DstPort == 443)
    .And(f => f.Tcp.Ack == true);
    
using var divert = new WinDivert(filter, WinDivertLayer.Network);
using var packet = new WinDivertPacket();
using var addr = new WinDivertAddress();

while(true)
{
    // 读包
    await divert.RecvAsync(packet, addr);

    // 解包
    var result = packet.GetParseResult();

    // 改包
    result.TcpHeader->DstPort = 443; 

    // 重算checksums
    packet.CalcChecksums(addr);

    // 修改后发出
    await divert.SendAsync(packet, addr);
}
