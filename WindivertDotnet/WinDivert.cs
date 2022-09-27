using System;
using System.ComponentModel;
using System.Diagnostics;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的操作对象
    /// </summary>
    [DebuggerDisplay("Filter = {Filter}, Layer = {Layer}")]
    public partial class WinDivert : IDisposable
    {
        private readonly WinDivertHandle handle;

        /// <summary>
        /// 获取过滤器
        /// </summary>
        public string Filter { get; }

        /// <summary>
        /// 获取工作层
        /// </summary>
        public WinDivertLayer Layer { get; }

        /// <summary>
        /// 获取软件版本
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        public Version Version
        {
            get
            {
                var major = (int)this.GetParam(WinDivertParam.VersionMajor);
                var minor = (int)this.GetParam(WinDivertParam.VersionMinor);
                return new Version(major, minor);
            }
        }

        /// <summary>
        /// 获取或设置列队的容量大小
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        public long QueueLength
        {
            get => this.GetParam(WinDivertParam.QueueLength);
            set => this.SetParam(WinDivertParam.QueueLength, value);
        }

        /// <summary>
        /// 获取或设自动丢弃数据包之前可以排队的最短时长
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        public TimeSpan QueueTime
        {
            get => TimeSpan.FromMilliseconds(this.GetParam(WinDivertParam.QueueTime));
            set => this.SetParam(WinDivertParam.QueueTime, (long)value.TotalMilliseconds);
        }

        /// <summary>
        /// 获取或设置存储在数据包队列中的最大字节数
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        public long QueueSize
        {
            get => this.GetParam(WinDivertParam.QueueSize);
            set => this.SetParam(WinDivertParam.QueueSize, value);
        }

        /// <summary>
        /// 创建一个WinDivert实例
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="layer">工作层</param>
        /// <param name="priority">优先级</param>
        /// <param name="flags">标记</param>
        /// <exception cref="Win32Exception"></exception>
        public WinDivert(Filter filter, WinDivertLayer layer, short priority = 0, WinDivertFlag flags = WinDivertFlag.None)
            : this(filter.ToString(), layer, priority, flags)
        {
        }

        /// <summary>
        /// 创建一个WinDivert实例
        /// </summary>
        /// <param name="filter">过滤器</param>
        /// <param name="layer">工作层</param>
        /// <param name="priority">优先级</param>
        /// <param name="flags">标记</param>
        /// <exception cref="Win32Exception"></exception>
        public WinDivert(string filter, WinDivertLayer layer, short priority = 0, WinDivertFlag flags = WinDivertFlag.None)
        {
            this.handle = WinDivertNative.WinDivertOpen(filter, layer, priority, flags);
            if (this.handle.IsInvalid)
            {
                throw new Win32Exception();
            }
            this.Filter = filter;
            this.Layer = layer;
        }

        /// <summary>
        /// 读取数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public int Recv(WinDivertPacket packet, ref WinDivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertRecv(this.handle, packet, packet.Capacity, ref length, ref addr);
            if (result == false)
            {
                throw new Win32Exception();
            }
            packet.Length = length;
            return length;
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public int Send(WinDivertPacket packet, ref WinDivertAddress addr)
        {
            var length = 0;
            var result = WinDivertNative.WinDivertSend(this.handle, packet, packet.Length, ref length, ref addr);
            if (result == false)
            {
                throw new Win32Exception();
            }
            packet.Length = length;
            return length;
        }

        /// <summary>
        /// 获取指定的参数值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        private long GetParam(WinDivertParam param)
        {
            var value = 0L;
            var result = WinDivertNative.WinDivertGetParam(this.handle, param, ref value);
            return result ? value : throw new Win32Exception();
        }

        /// <summary>
        /// 设置指定的参数值
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <exception cref="Win32Exception"></exception>
        private void SetParam(WinDivertParam param, long value)
        {
            if (WinDivertNative.WinDivertSetParam(this.handle, param, value) == false)
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="how"></param>
        /// <returns></returns>
        public bool Shutdown(WinDivertShutdown how)
        {
            return WinDivertNative.WinDivertShutdown(this.handle, how);
        }

        /// <summary>
        /// 关闭并释放资源
        /// </summary>
        public void Dispose()
        {
            this.Shutdown(WinDivertShutdown.Both);
            this.handle.Dispose();
        }
    }
}
