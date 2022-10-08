using WindivertDotnet;

namespace App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var filter = Filter.True.And(f => f.IsUdp);
            using var divert = new WinDivert(filter, WinDivertLayer.Network);
            using var packet = new WinDivertPacket();
            var addr = new WinDivertAddress();

            while (true)
            {
                var recvLength = await divert.RecvAsync(packet, ref addr);
                var result = packet.GetParseResult();

                var checkState = packet.CalcChecksums(ref addr);
               // var sendLength = divert.Send(packet, ref addr);

                Console.WriteLine(result.Protocol);
            }
        }
    }
}