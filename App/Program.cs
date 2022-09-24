using System.ComponentModel.DataAnnotations;
using WindivertDotnet;

namespace App
{
    internal class Program
    {
        unsafe static void Main(string[] args)
        {
            using var win = new WinDivert("udp.DstPort == 53", WINDIVERT_LAYER.NETWORK, 0, WINDIVERT_FLAG.RECV_ONLY);
            using var packet = new WinDivertPacket();
            var addr = new WindivertAddress();
            var b = win.WinDivertRecv(packet, ref addr);
            var result = packet.GetParseResult();
            var x = result.IPV4Header->SrcAddr;

            Console.WriteLine("Hello, World!");
        }
    }
}