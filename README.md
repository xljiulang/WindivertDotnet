# WindivertDotnet
面向对象的Windivert的dotnet封装


### 如何使用
```
using var divert = new WinDivert("true", WinDivertLayer.Network);
using var packet = new WinDivertPacket();
var addr = new WinDivertAddress();

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

