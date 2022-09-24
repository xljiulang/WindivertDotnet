using System;
using System.ComponentModel;
using System.Net.Sockets;

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
        public WinDivert(string filter, WINDIVERT_LAYER layer, short priority, WINDIVERT_FLAG flags)
        {
            this.handle = WinDivertNative.WinDivertOpen(filter, layer, priority, flags);
            if (this.handle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }


        public bool WinDivertRecv(
            WinDivertPacket packet,
            ref WindivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertRecv(this.handle, packet.Handle, packet.Capacity, ref length, ref addr);
            packet.Length = length;
            return result;
        } 
       

        public void Dispose()
        {
            this.handle.Dispose();
        }
    }
}
