using System;
using System.Threading.Tasks;
using WindivertDotnet;

namespace App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var filter = Filter.True.And(f => f.IsIP && f.IsTcp);
            using var divert = new WinDivert(filter, WinDivertLayer.Network);
            using var packet = new WinDivertPacket();
            using var addr = new WinDivertAddress();

            while (true)
            {
                var recvLength = await divert.RecvAsync(packet, addr);
                var result = packet.GetParseResult();

                var checkState = packet.CalcChecksums(addr);
                var sendLength = await divert.SendAsync(packet, addr);

                Console.WriteLine($"{result.Protocol} {recvLength} {sendLength}");
            }
        }
    }
}