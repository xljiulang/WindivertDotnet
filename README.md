# WindivertDotnet
面向对象的Windivert的dotnet封装

### nuget
[WindivertDotnet](https://www.nuget.org/packages/WindivertDotnet)

### 如何使用
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
