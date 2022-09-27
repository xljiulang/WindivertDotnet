using System.Net;
using System.Net.Sockets;
using WindivertDotnet;

namespace App
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            var filter = Filter.True
                .And(f => f.Ip.SrcAddr == IPAddress.Loopback.ToString())
                .And(f => f.Tcp.DstPort == 443)
                .And(f => f.Tcp.Ack == true);

            using var divert = new WinDivert(filter, WinDivertLayer.Network);
            using var packet = new WinDivertPacket();
            var addr = new WinDivertAddress();

            divert.Recv(packet, ref addr);
            var result = packet.GetParseResult();

            packet.CalcChecksums(ref addr);
            divert.Send(packet, ref addr);

            Console.WriteLine("Hello, World!");
        }
    }
}