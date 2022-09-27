using System.Linq.Expressions;
using System.Net;
using WindivertDotnet;

namespace App
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            using var divert = new WinDivert("(((((true and (( tcp )))) or ((ip.SrcAddr == 127.0.0.1)))) and tcp.Rst)", WinDivertLayer.Network);
            // using var packet = new WinDivertPacket();
            // var addr = new WinDivertAddress();

            var filter = Filter
                .True()
                .And(f => (!f.TcpProtocol == false))
                .Or(f => f.Ip.SrcAddr == IPAddress.Loopback.ToString())
                .And(item => item.Tcp.Rst)
                .ToFilter();

            //divert.Recv(packet, ref addr);
            //var result = packet.GetParseResult();

            //packet.CalcChecksums(ref addr);
            //divert.Send(packet, ref addr);

            Console.WriteLine("Hello, World!");
        }
    }
}