using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的操作对象
    /// </summary>
    [DebuggerDisplay("Filter = {Filter}, Layer = {Layer}")]
    public partial class WinDivert : IDisposable
    {
        private readonly WinDivertHandle handle;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ThreadPoolBoundHandle> boundHandle;

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
        public Version Version { get; }


        /// <summary>
        /// 获取或设置列队的容量大小
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public long QueueLength
        {
            get => this.GetParam(WinDivertParam.QueueLength);
            set => this.SetParam(WinDivertParam.QueueLength, value);
        }

        /// <summary>
        /// 获取或设自动丢弃数据包之前可以排队的最短时长
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public TimeSpan QueueTime
        {
            get => TimeSpan.FromMilliseconds(this.GetParam(WinDivertParam.QueueTime));
            set => this.SetParam(WinDivertParam.QueueTime, (long)value.TotalMilliseconds);
        }

        /// <summary>
        /// 获取或设置存储在数据包队列中的最大字节数
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public long QueueSize
        {
            get => this.GetParam(WinDivertParam.QueueSize);
            set => this.SetParam(WinDivertParam.QueueSize, value);
        }

        /// <summary>
        /// 创建一个WinDivert实例
        /// </summary>
        /// <param name="filter">过滤器 https://reqrypt.org/windivert-doc.html#filter_language</param>
        /// <param name="layer">工作层</param>
        /// <param name="priority">优先级，越小越高</param>
        /// <param name="flags">标记</param>
        /// <exception cref="Win32Exception"></exception>
        public WinDivert(Filter filter, WinDivertLayer layer, short priority = 0, WinDivertFlag flags = WinDivertFlag.None)
            : this(filter.ToString(), layer, priority, flags)
        {
        }

        /// <summary>
        /// 创建一个WinDivert实例
        /// </summary>
        /// <param name="filter">过滤器 https://reqrypt.org/windivert-doc.html#filter_language</param>
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
            this.boundHandle = new Lazy<ThreadPoolBoundHandle>(() => ThreadPoolBoundHandle.BindHandle(this.handle));

            this.Filter = filter;
            this.Layer = layer;

            var major = this.GetParam(WinDivertParam.VersionMajor);
            var minor = this.GetParam(WinDivertParam.VersionMinor);
            this.Version = new Version((int)major, (int)minor);
        }

        /// <summary>
        /// 同步阻塞读取数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public int Recv(WinDivertPacket packet, WinDivertAddress addr)
        {
            return this.RecvAsync(packet, addr).AsTask().Result;
        }

        /// <summary>
        /// 异步IO读取数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public ValueTask<int> RecvAsync(WinDivertPacket packet, WinDivertAddress addr)
        {
            var operation = new WindivertRecvOperation(this.handle, packet, addr, this.boundHandle.Value);
            return operation.IOControl();
        }

        /// <summary>
        /// 同步阻塞发送数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public int Send(WinDivertPacket packet, WinDivertAddress addr)
        {
            return this.SendAsync(packet, addr).AsTask().Result;
        }

        /// <summary>
        /// 异步IO发送数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public ValueTask<int> SendAsync(WinDivertPacket packet, WinDivertAddress addr)
        {
            var operation = new WindivertSendOperation(this.handle, packet, addr, this.boundHandle.Value);
            return operation.IOControl();
        }

        /// <summary>
        /// 获取指定的参数值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private long GetParam(WinDivertParam param)
        {
            if (this.boundHandle.IsValueCreated)
            {
                throw new InvalidOperationException();
            }

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
        /// <exception cref="InvalidOperationException"></exception>
        private void SetParam(WinDivertParam param, long value)
        {
            if (this.boundHandle.IsValueCreated)
            {
                throw new InvalidOperationException();
            }

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
            if (this.boundHandle.IsValueCreated)
            {
                this.boundHandle.Value.Dispose();
            }
        }
    }
}
