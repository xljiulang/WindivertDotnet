# WindivertDotnet
面向对象的[WinDivertv2.2](https://github.com/basil00/Divert)的dotnet异步封装，轻松实现网络数据拦截与修改。

### 1 nuget
[WindivertDotnet](https://www.nuget.org/packages/WindivertDotnet)
```
<PackageReference Include="WindivertDotnet" Version="1.*" /> 
```

### 2 功能介绍
* Filter对象支持Lambda构建filter language，脱离字符串的苦海；
* 内存安全的WinDivert对象，基于IOCP的ValueTask异步发送与接收方法；
* 内存安全的WinDivertPacket对象，提供获取包有效数据长度、解包、重构chucksums等；
* WinDivertParseResult提供对解包的数据进行精细修改，修改后对WinDivertPacket直接生效；

### 3 Api表
#### 3.1 WinDivert
| Api                                                                            | 描述               |
| ------------------------------------------------------------------------------ | ------------------ |
| int Recv(WinDivertPacket, WinDivertAddress)                                    | 同步阻塞读取数据包 |
| int Recv(WinDivertPacket, WinDivertAddress, CancellationToken)                 | 同步阻塞读取数据包 |
| `ValueTask<int>` RecvAsync(WinDivertPacket, WinDivertAddress)                    | 异步IO读取数据包   |
| `ValueTask<int>` RecvAsync(WinDivertPacket, WinDivertAddress, CancellationToken) | 异步IO读取数据包   |
| int Send(WinDivertPacket, WinDivertAddress)                                    | 同步阻塞发送数据包 |
| int Send(WinDivertPacket, WinDivertAddress, CancellationToken)                 | 同步阻塞发送数据包 |
| `ValueTask<int>` SendAsync(WinDivertPacket, WinDivertAddress)                    | 异步IO发送数据包   |
| `ValueTask<int>` SendAsync(WinDivertPacket, WinDivertAddress, CancellationToken) | 异步IO发送数据包   |

#### 3.2 Filter
| Api                                           | 描述                      |
| --------------------------------------------- | ------------------------- |
| static string Format(string, WinDivertLayer)  | 格式化filter              |
| static string Compile(string, WinDivertLayer) | 编译filter                |
| static Filter True { get; }                   | 获取值为true的filter      |
| static Filter False { get; }                  | 获取值为false的filter     |
| Filter And(Expression<Func<IFilter, bool>>)   | 使用and逻辑创建新的fitler |
| Filter And(Filter)                           | 使用and逻辑创建新的fitler |
| Filter Or(Expression<Func<IFilter, bool>>)    | 使用or逻辑创建新的fitler  |
| Filter Or(Filter)                            | 使用or逻辑创建新的fitler  |

#### 3.3 WinDivertPacket
| Api                                                 | 描述                                                            |
| --------------------------------------------------- | --------------------------------------------------------------- |
| int Capacity { get; }                               | 获取缓冲区容量                                                  |
| int Length { get; set;}                             | 获取或设置有效数据的长度                                        |
| Span<byte> Span { get; }                            | 获取有效数据视图                                                |
| void Clear()                                        | 将有效数据清0                                                   |
| Span<byte> GetSpan(int, int)                        | 获取缓冲区的Span                                                |
| bool CalcChecksums(WinDivertAddress, ChecksumsFlag) | 重新计算和修改相关的Checksums                                   |
| bool CalcNetworkIfIdx(WinDivertAddress )            | 根据IP地址重新计算和修改addr的Network->IfIdx                    |
| bool CalcOutboundFlag(WinDivertAddress)             | 根据IP地址和addr.Network->IfIdx重新计算和修改addr的Outbound标记 |
| bool CalcLoopbackFlag(WinDivertAddress)             | 根据IP地址重新计算和修改addr的Loopback标记                      |
| bool DecrementTTL()                                 | ttl减1                                                          |
| int GetHashCode()                                   | 获取包的哈希                                                    |
| int GetHashCode(long)                               | 获取包的哈希                                                    |
| WinDivertParseResult GetParseResult()               | 获取包的解析结果                                                |

#### 3.4 WinDivertRouter
| Api                                        | 描述               |
| ------------------------------------------ | ------------------ |
| IPAddress DstAddress { get; }              | 获取目标地址       |
| IPAddress SrcAddress { get; }              | 获取源地址         |
| int InterfaceIndex { get; }                | 获取网络接口索引   |
| bool IsOutbound { get; }                   | 获取是否为出口方向 |
| WinDivertRouter(IPAddress)                 | 构造器             |
| WinDivertRouter(IPAddress, IPAddress)      | 构造器             |
| WinDivertRouter(IPAddress, IPAddress, int) | 构造器             |


### 4 如何使用
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
