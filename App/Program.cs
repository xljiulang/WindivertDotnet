using WindivertDotnet;

namespace App
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            using var win = new WinDivert("true", WinDivertLayer.Network);
            using var packet = new WinDivertPacket();
            var addr = new WinDivertAddress();
            var b = win.WinDivertRecv(packet, ref addr);
            var result = packet.GetParseResult();

            Console.WriteLine("Hello, World!");
        }
    }
}