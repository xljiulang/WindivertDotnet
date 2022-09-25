using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    public partial class WinDivert : IDisposable
    {
        private readonly WinDivertHandle handle;

        public SafeHandle Handle => this.handle;

        public Version Version
        {
            get
            {
                var major = (int)this.GetParam(WinDivertParam.VersionMajor);
                var minor = (int)this.GetParam(WinDivertParam.VersionMinor);
                return new Version(major, minor);
            }
        }


        public static WinDivert LayerNetwork(string filter, short priority = 0, WinDivertFlag flag = WinDivertFlag.None)
        {
            return new WinDivert(filter, WinDivertLayer.Network, priority, flag);
        }

        public static WinDivert LayerNetworkForward(string filter, short priority = 0, WinDivertFlag flag = WinDivertFlag.None)
        {
            return new WinDivert(filter, WinDivertLayer.NetworkForward, priority, flag);
        }

        public static WinDivert LayerFlow(string filter, short priority = 0, WinDivertFlag flag = WinDivertFlag.Sniff | WinDivertFlag.RecvOnly)
        {
            return new WinDivert(filter, WinDivertLayer.Flow, priority, flag);
        }
        public static WinDivert LayerSocket(string filter, short priority = 0, WinDivertFlag flag = WinDivertFlag.RecvOnly)
        {
            return new WinDivert(filter, WinDivertLayer.Socket, priority, flag);
        }

        public static WinDivert LayerReflect(string filter, short priority = 0, WinDivertFlag flag = WinDivertFlag.Sniff | WinDivertFlag.RecvOnly)
        {
            return new WinDivert(filter, WinDivertLayer.Reflect, priority, flag);
        }

        /// <summary>
        /// Win32Exception
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="layer"></param>
        /// <param name="priority"></param>
        /// <param name="flags"></param>
        /// <exception cref="Win32Exception"></exception>
        public WinDivert(string filter, WinDivertLayer layer, short priority, WinDivertFlag flags)
        {
            this.handle = WinDivertNative.WinDivertOpen(filter, layer, priority, flags);
            if (this.handle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }

        public int Recv(WinDivertPacket packet, ref WinDivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertRecv(this.handle, packet.Handle, packet.Capacity, ref length, ref addr);
            if (result == false)
            {
                throw new Win32Exception();
            }
            packet.Length = length;
            return length;
        }

        public int Send(WinDivertPacket packet, ref WinDivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertSend(this.handle, packet.Handle, packet.Length, ref length, ref addr);
            if (result == false)
            {
                throw new Win32Exception();
            }
            packet.Length = length;
            return length;
        }

        public long GetParam(WinDivertParam param)
        {
            var value = 0L;
            var result = WinDivertNative.WinDivertGetParam(this.handle, param, ref value);
            return result ? value : throw new Win32Exception();
        }

        public bool SetParam(WinDivertParam param, long value)
        {
            return WinDivertNative.WinDivertSetParam(this.handle, param, value);
        }

        public bool Shutdown(WinDivertShutdown how)
        {
            return WinDivertNative.WinDivertShutdown(this.handle, how);
        }

        public void Dispose()
        {
            this.Shutdown(WinDivertShutdown.Both);
            this.handle.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(WinDivert)} v{this.Version}";
        }
    }
}
