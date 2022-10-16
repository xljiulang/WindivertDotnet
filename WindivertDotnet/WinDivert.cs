using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert的操作对象
    /// </summary>
    [SupportedOSPlatform("windows")]
    [DebuggerDisplay("Filter = {Filter}, Layer = {Layer}")]
    public class WinDivert : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ThreadPoolBoundHandle> boundHandleLazy;

        /// <summary>
        /// 获取过滤器
        /// </summary>
        public string Filter { get; }

        /// <summary>
        /// 获取工作层
        /// </summary>
        public WinDivertLayer Layer { get; }

        /// <summary>
        /// 获取优先级
        /// </summary>
        public short Priority { get; }

        /// <summary>
        /// 获取工作方式标志
        /// </summary>
        public WinDivertFlag Flags { get; }

        /// <summary>
        /// 获取软件版本
        /// </summary>
        public Version Version { get; }


        /// <summary>
        /// 获取或设置列队的容量大小
        /// 默认为4096，范围为[32,16384]
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
        /// 默认为2s，范围为100ms到16s
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
        /// 默认4MB，范围为64KB到32MB
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
        /// <param name="priority">优先级，值越大优先级越高[-30000,30000]</param>
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
        /// <param name="priority">优先级，值越大优先级越高[-30000,30000]</param>
        /// <param name="flags">标记</param>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public WinDivert(string filter, WinDivertLayer layer, short priority = 0, WinDivertFlag flags = WinDivertFlag.None)
            : base(ownsHandle: true)
        {
            var compileFilter = WindivertDotnet.Filter.Compile(filter, layer);

            if (Enum.IsDefined(typeof(WinDivertLayer), layer) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(layer));
            }

            if (priority < -30000 || priority > 30000)
            {
                throw new ArgumentOutOfRangeException(nameof(priority), "值范围为[-30000,30000]");
            }

            this.handle = WinDivertNative.WinDivertOpen(compileFilter, layer, priority, flags);
            this.boundHandleLazy = new Lazy<ThreadPoolBoundHandle>(() => ThreadPoolBoundHandle.BindHandle(this));

            if (this.IsInvalid == true)
            {
                throw new Win32Exception();
            }

            this.Filter = WindivertDotnet.Filter.Format(filter, layer);
            this.Layer = layer;
            this.Priority = priority;
            this.Flags = flags;
            this.Version = this.GetVersion();
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns></returns>
        private Version GetVersion()
        {
            var major = this.GetParam(WinDivertParam.VersionMajor);
            var minor = this.GetParam(WinDivertParam.VersionMinor);
            return new Version((int)major, (int)minor);
        }

        /// <summary>
        /// 获取线程池绑定句柄
        /// </summary>
        /// <returns></returns>
        internal ThreadPoolBoundHandle GetThreadPoolBoundHandle()
        {
            return this.boundHandleLazy.Value;
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
            return this.RecvAsync(packet, addr).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 异步IO读取数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public async ValueTask<int> RecvAsync(WinDivertPacket packet, WinDivertAddress addr)
        {
            using var operation = new WinDivertRecvOperation(this, packet, addr);
            return await operation.IOControlAsync();
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
            return this.SendAsync(packet, addr).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 异步IO发送数据包
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="addr">地址信息</param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        public async ValueTask<int> SendAsync(WinDivertPacket packet, WinDivertAddress addr)
        {
            using var operation = new WinDivertSendOperation(this, packet, addr);
            return await operation.IOControlAsync();
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
            if (this.boundHandleLazy.IsValueCreated)
            {
                throw new InvalidOperationException();
            }

            var value = 0L;
            var status = WinDivertNative.WinDivertGetParam(this, param, ref value);
            return status ? value : throw new Win32Exception();
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
            if (this.boundHandleLazy.IsValueCreated)
            {
                throw new InvalidOperationException();
            }

            if (param == WinDivertParam.QueueSize)
            {
                if (value < 65535L || value > 33554432L)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

            if (param == WinDivertParam.QueueTime)
            {
                if (value < 100L || value > 16000L)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

            if (param == WinDivertParam.QueueLength)
            {
                if (value < 32L || value > 16384L)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }

            if (WinDivertNative.WinDivertSetParam(this, param, value) == false)
            {
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="how">关闭方式</param>
        /// <returns></returns>
        public bool Shutdown(WinDivertShutdown how = WinDivertShutdown.Both)
        {
            return WinDivertNative.WinDivertShutdown(this, how);
        }

        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            return WinDivertNative.WinDivertClose(this.handle);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.boundHandleLazy.IsValueCreated)
            {
                this.boundHandleLazy.Value.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
