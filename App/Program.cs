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

            divert.Recv(packet, ref addr);
            var result = packet.GetParseResult();

            packet.CalcChecksums(ref addr);
            divert.Send(packet, ref addr);

            Console.WriteLine("Hello, World!");
        }
    }
}