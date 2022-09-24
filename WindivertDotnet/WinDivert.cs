using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WindivertDotnet
{
    public partial class WinDivert : IDisposable
    {
        private readonly WinDivertHandle handle;

        /// <summary>
        /// Win32Exception
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="flags"></param>
        /// <exception cref="Win32Exception"></exception>
        public WinDivert(string filter, WinDivertLayer layer, short priority = 0, WinDivertFlag flags = WinDivertFlag.Read)
        {
            this.handle = WinDivertNative.WinDivertOpen(filter, layer, priority, flags);
            if (this.handle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }


        public bool WinDivertRecv(WinDivertPacket packet, ref WinDivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertRecv(this.handle, packet.Handle, packet.Capacity, ref length, ref addr);
            packet.Length = length;
            return result;
        }

        public Task<bool> WinDivertRecvAsync(WinDivertPacket packet, ref WinDivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertRecv(this.handle, packet.Handle, packet.Capacity, ref length, ref addr);
            packet.Length = length;
            return Task.FromResult(true);
        }


        public void Dispose()
        {
            this.handle.Dispose();
        }
    }
}
