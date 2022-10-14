namespace System.Runtime.InteropServices
{
    /// <summary>
    /// 提供非托管内容的申请和释放
    /// </summary>
    static unsafe class MemoryNative
    {
#if NET6_0_OR_GREATER
        public static IntPtr AllocZeroed(int size)
        {
            var pointer = NativeMemory.AllocZeroed((uint)size);
            return new IntPtr(pointer);
        }

        public static void Free(IntPtr handle)
        {
            NativeMemory.Free(handle.ToPointer());
        }
#else
        public static IntPtr AllocZeroed(int size)
        {
            var handle = Marshal.AllocHGlobal(size);
            new Span<byte>(handle.ToPointer(), size).Clear();
            return handle;
        }

        public static void Free(IntPtr handle)
        {
            Marshal.FreeHGlobal(handle);
        }
#endif
    }
}
