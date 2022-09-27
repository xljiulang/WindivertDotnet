using System.Linq.Expressions;
using WindivertDotnet;

namespace App
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            using var divert = new WinDivert("true", WinDivertLayer.Network);
            using var packet = new WinDivertPacket();
            var addr = new WinDivertAddress();

            var filter = Filter.True().And(f => f.IsUdp == true).And(item => item.Tcp.Rst).ToFilter();

            //divert.Recv(packet, ref addr);
            //var result = packet.GetParseResult();

            //packet.CalcChecksums(ref addr);
            //divert.Send(packet, ref addr);

            Console.WriteLine("Hello, World!");
        }
    }
}