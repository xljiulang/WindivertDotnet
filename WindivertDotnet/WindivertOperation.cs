using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace WindivertDotnet
{
    /// <summary>
    /// Windivert控制器
    /// </summary>
    abstract unsafe class WindivertOperation : IDisposable
    {
        private readonly ThreadPoolBoundHandle boundHandle;
        private readonly NativeOverlapped* nativeOverlapped;

        private readonly ValueTaskCompletionSource<int> taskCompletionSource = new();
        private static readonly IOCompletionCallback completionCallback = new(IOCompletionCallback);

        /// <summary>
        /// Windivert控制器
        /// </summary>
        /// <param name="divert"></param> 
        public WindivertOperation(WinDivert divert)
        {
            this.boundHandle = divert.GetThreadPoolBoundHandle();
            this.nativeOverlapped = this.boundHandle.AllocateNativeOverlapped(completionCallback, this, null);
        }

        /// <summary>
        /// io控制
        /// </summary> 
        /// <exception cref="Win32Exception"></exception>
        public ValueTask<int> IOControlAsync()
        {
            var length = 0;
            // 如果触发异步回调，回调里不会反写pLength，所以这里可以指向栈内存的length
            if (this.IOControl(&length, this.nativeOverlapped))
            {
                this.SetResult(length);
            }

            return this.taskCompletionSource.Task;
        }

        /// <summary>
        /// io控制
        /// </summary>
        /// <param name="pLength"></param> 
        /// <param name="nativeOverlapped"></param>
        /// <returns></returns>
        protected abstract bool IOControl(int* pLength, NativeOverlapped* nativeOverlapped);

        /// <summary>
        /// io完成回调
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="numBytes"></param>
        /// <param name="pOVERLAP"></param>
        private static void IOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
        {
            var operation = (WindivertOperation)ThreadPoolBoundHandle.GetNativeOverlappedState(pOVERLAP);
            if (errorCode > 0)
            {
                operation.SetException((int)errorCode);
            }
            else
            {
                operation.SetResult((int)numBytes);
            }
        }

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="length"></param>
        protected virtual void SetResult(int length)
        {
            this.taskCompletionSource.SetResult(length);
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        /// <param name="errorCode"></param>
        protected virtual void SetException(int errorCode)
        {
            var exception = new Win32Exception(errorCode);
            this.taskCompletionSource.SetException(exception);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            this.boundHandle.FreeNativeOverlapped(this.nativeOverlapped);
        }


        private class ValueTaskCompletionSource<T> : IValueTaskSource<T>
        {
            private ManualResetValueTaskSourceCore<T> core;

            public ValueTask<T> Task => new(this, this.core.Version);

            public void SetResult(T result)
            {
                this.core.SetResult(result);
            }

            public void SetException(Exception error)
            {
                this.core.SetException(error);
            }

            T IValueTaskSource<T>.GetResult(short token)
            {
                return this.core.GetResult(token);
            }

            ValueTaskSourceStatus IValueTaskSource<T>.GetStatus(short token)
            {
                return this.core.GetStatus(token);
            }

            void IValueTaskSource<T>.OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
            {
                this.core.OnCompleted(continuation, state, token, flags);
            }
        }
    }
}
