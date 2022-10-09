using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace WindivertDotnet
{
    /// <summary>
    /// Windivert控制器
    /// </summary>
    abstract class WindivertController
    {
        private readonly ThreadPoolBoundHandle boundHandle;
        private readonly PreAllocatedOverlapped preAllocatedOverlapped;
        private readonly TaskCompletionSource<int> taskCompletionSource = new();

        protected const int ERROR_IO_PENDING = 997;

        /// <summary>
        /// 获取io重叠对象
        /// </summary>
        protected unsafe NativeOverlapped* NativeOverlapped { get; }

        /// <summary>
        /// 获取操作任务
        /// </summary>
        public Task<int> Task => this.taskCompletionSource.Task;

        /// <summary>
        /// Windivert控制器
        /// </summary>
        /// <param name="boundHandle"></param>
        /// <param name="completionCallback"></param>
        public unsafe WindivertController(
            ThreadPoolBoundHandle boundHandle,
            IOCompletionCallback completionCallback)
        {
            this.boundHandle = boundHandle;
            this.preAllocatedOverlapped = new PreAllocatedOverlapped(completionCallback, this, null);
            this.NativeOverlapped = this.boundHandle.AllocateNativeOverlapped(this.preAllocatedOverlapped);
        }

        /// <summary>
        /// io控制
        /// </summary>
        /// <param name="addr"></param>
        public abstract void IoControl(ref WinDivertAddress addr);

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="length"></param>
        public virtual void SetResult(int length)
        {
            this.FreeNative();
            this.taskCompletionSource.SetResult(length);
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        /// <param name="errorCode"></param>
        public virtual void SetException(int errorCode)
        {
            this.FreeNative();
            var exception = new Win32Exception(errorCode);
            this.taskCompletionSource.SetException(exception);
        }

        /// <summary>
        /// 释放非托管资源
        /// </summary>
        private unsafe void FreeNative()
        {
            this.boundHandle.FreeNativeOverlapped(this.NativeOverlapped);
            this.preAllocatedOverlapped.Dispose();
        }
    }
}
