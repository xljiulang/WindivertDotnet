using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace WindivertDotnet
{
    /// <summary>
    /// Windivert控制器
    /// </summary>
    abstract class WindivertOperation
    {
        private readonly ThreadPoolBoundHandle boundHandle;
        private readonly unsafe NativeOverlapped* nativeOverlapped;
        private readonly unsafe int* pLength;

        private readonly ValueTaskCompletionSource<int> taskCompletionSource = new();
        private static readonly unsafe IOCompletionCallback completionCallback = new(IOCompletionCallback);

        /// <summary>
        /// 获取操作任务
        /// </summary>
        public ValueTask<int> Task => this.taskCompletionSource.Task;

        /// <summary>
        /// Windivert控制器
        /// </summary>
        /// <param name="boundHandle"></param> 
        public unsafe WindivertOperation(ThreadPoolBoundHandle boundHandle)
        {
            this.boundHandle = boundHandle;
            this.nativeOverlapped = this.boundHandle.AllocateNativeOverlapped(completionCallback, this, null);
            this.pLength = (int*)Marshal.AllocHGlobal(sizeof(int));
        }

        /// <summary>
        /// io控制
        /// </summary>
        /// <param name="addr"></param>
        public unsafe void IOControl(ref WinDivertAddress addr)
        {
            if (this.IOControl(this.pLength, ref addr, this.nativeOverlapped))
            {
                this.SetResult(*this.pLength);
            }
        }

        /// <summary>
        /// io控制
        /// </summary>
        /// <param name="pLength"></param>
        /// <param name="addr"></param>
        /// <param name="nativeOverlapped"></param>
        /// <returns></returns>
        protected unsafe abstract bool IOControl(int* pLength, ref WinDivertAddress addr, NativeOverlapped* nativeOverlapped);

        /// <summary>
        /// io完成回调
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="numBytes"></param>
        /// <param name="pOVERLAP"></param>
        private unsafe static void IOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP)
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
            this.FreeNative();
            this.taskCompletionSource.SetResult(length);
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        /// <param name="errorCode"></param>
        protected virtual void SetException(int errorCode)
        {
            this.FreeNative();
            var exception = new Win32Exception(errorCode);
            this.taskCompletionSource.SetException(exception);
        }

        /// <summary>
        /// 释放本机资源
        /// </summary>
        protected virtual unsafe void FreeNative()
        {
            Marshal.FreeHGlobal(new IntPtr(this.pLength));
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
