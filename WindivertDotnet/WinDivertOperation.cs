using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace WindivertDotnet
{
    /// <summary>
    /// Windivert控制器
    /// </summary>
    [SupportedOSPlatform("windows")]
    abstract class WinDivertOperation : IDisposable, IValueTaskSource<int>
    {
        protected readonly WinDivert divert;
        private readonly unsafe NativeOverlapped* nativeOverlapped;

        private ManualResetValueTaskSourceCore<int> taskSource; // 不能readonly 
        private static readonly unsafe IOCompletionCallback completionCallback = new(IOCompletionCallback);


        /// <summary>
        /// Windivert控制器
        /// </summary>
        /// <param name="divert"></param> 
        public unsafe WinDivertOperation(WinDivert divert)
        {
            this.divert = divert;
            this.nativeOverlapped = divert.GetThreadPoolBoundHandle().AllocateNativeOverlapped(completionCallback, this, null);
        }

        /// <summary>
        /// io控制
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async ValueTask<int> IOControlAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (cancellationToken.Register(CancelIoEx))
            {
                return await IOControlAsync();
            }

            unsafe void CancelIoEx()
            {
                Kernel32Native.CancelIoEx(this.divert, this.nativeOverlapped);
            }

            unsafe ValueTask<int> IOControlAsync()
            {
                var length = 0; // 如果触发异步回调，回调里不会反写pLength，所以这里可以声明为方法内部变量
                return this.IOControl(&length, this.nativeOverlapped)
                    ? new ValueTask<int>(length)
                    : new ValueTask<int>(this, this.taskSource.Version);
            }
        }

        /// <summary>
        /// io控制
        /// </summary>
        /// <param name="pLength"></param>
        /// <param name="nativeOverlapped"></param>
        /// <returns></returns>
        protected abstract unsafe bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped);

        /// <summary>
        /// io完成回调
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="numBytes"></param>
        /// <param name="pOVERLAP"></param>
        private static unsafe void IOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
        {
            var operation = (WinDivertOperation)ThreadPoolBoundHandle.GetNativeOverlappedState(pOVERLAP)!;
            if (errorCode == 0)
            {
                operation.taskSource.SetResult((int)numBytes);
            }
            else if (errorCode == 995) // ERROR_OPERATION_ABORTED
            {
                var exception = new TaskCanceledException();
                operation.taskSource.SetException(exception);
            }
            else
            {
                var exception = new Win32Exception((int)errorCode);
                operation.taskSource.SetException(exception);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual unsafe void Dispose()
        {
            this.divert.GetThreadPoolBoundHandle().FreeNativeOverlapped(this.nativeOverlapped);
        }

        int IValueTaskSource<int>.GetResult(short token)
        {
            return this.taskSource.GetResult(token);
        }

        ValueTaskSourceStatus IValueTaskSource<int>.GetStatus(short token)
        {
            return this.taskSource.GetStatus(token);
        }

        void IValueTaskSource<int>.OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            this.taskSource.OnCompleted(continuation, state, token, flags);
        }
    }
}
