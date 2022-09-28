# WindivertDotnet
面向对象的[Windivertv2.2](https://reqrypt.org/windivert-doc.html)的dotnet封装，轻松实现网络数据拦截与修改。

### 1 nuget
[WindivertDotnet](https://www.nuget.org/packages/WindivertDotnet)

### 2 功能介绍
* Filter对象支持Labda构建filter language，脱离字符串的苦海；
* WinDivert对象自动维护Windivert句柄，提供接收包与发送包方法；
* WinDivertPacket提供获取包有效数据长度、解包、重构chucksums等；
* WinDivertParseResult提供对解包的数据进行精细修改，修改后对Packet直接生效；

### 3 如何使用
```
var filter = Filter.True
    .And(f => f.Network.Loopback)
    .And(f => f.Tcp.DstPort == 443)
    .And(f => f.Tcp.Ack == true);
    
using var divert = new WinDivert(filter, WinDivertLayer.Network);
using var packet = new WinDivertPacket();
var addr = new WinDivertAddress();

while(true)
{
    // 读包
    divert.Recv(packet, ref addr);

    // 解包
    var result = packet.GetParseResult();

    // 改包
    result.TcpHeader->DstPort = 443; 

    // 重算checksums
    packet.CalcChecksums(ref addr);

    // 修改后发出
    divert.Send(packet, ref addr);
}
