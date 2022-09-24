using WindivertDotnet;

namespace App
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            using var win = new WinDivert("udp.DstPort == 53", WindivertLayer.Network, 0, WindivertFlag.ReadWrite);
            using var packet = new WinDivertPacket();
            var addr = new WindivertAddress();
            var b = win.WinDivertRecv(packet, ref addr);
            var result = packet.GetParseResult();

            Console.WriteLine("Hello, World!");
        }
    }
}