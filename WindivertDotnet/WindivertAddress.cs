using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindivertDotnet
{
    /// <summary>
    /// 表示WinDivert地址信息
    /// </summary>
    [DebuggerDisplay("Flags = {Flags}")]
    public unsafe sealed class WinDivertAddress : IDisposable
    {
        private readonly WinDivertAddressBuffer buffer = new();

        /// <summary>
        /// 获取结构大小
        /// </summary>
        public static int Size { get; } = sizeof(WinDivertAddressStruct);

        /// <summary>
        /// 发生的时间戳
        /// </summary>
        public long Timestamp
        {
            get => this.buffer.Pointer->Timestamp;
            set => this.buffer.Pointer->Timestamp = value;
        }

        /// <summary>
        /// 所在层
        /// </summary>
        public WinDivertLayer Layer
        {
            get => this.buffer.Pointer->Layer;
            set => this.buffer.Pointer->Layer = value;
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        public WinDivertEvent Event
        {
            get => this.buffer.Pointer->Event;
            set => this.buffer.Pointer->Event = value;
        }

        /// <summary>
        /// 标记
        /// </summary>
        public WinDivertAddressFlag Flags
        {
            get => this.buffer.Pointer->Flags;
            set => this.buffer.Pointer->Flags = value;
        }

        /// <summary>
        /// 保留
        /// </summary>
        public byte Reserved
        {
            get => this.buffer.Pointer->Reserved;
            set => this.buffer.Pointer->Reserved = value;
        }

        /// <summary>
        /// 保留
        /// </summary>
        public uint Reserved2
        {
            get => this.buffer.Pointer->Reserved2;
            set => this.buffer.Pointer->Reserved2 = value;
        }

        /// <summary>
        /// Network信息
        /// </summary>
        public WinDivertDataNetwork Network
        {
            get => this.buffer.Pointer->Network;
            set => this.buffer.Pointer->Network = value;
        }

        /// <summary>
        /// Flow信息
        /// </summary>
        public WinDivertDataFlow Flow
        {
            get => this.buffer.Pointer->Flow;
            set => this.buffer.Pointer->Flow = value;
        }

        /// <summary>
        /// Socket信息
        /// </summary>
        public WinDivertDataSocket Socket
        {
            get => this.buffer.Pointer->Socket;
            set => this.buffer.Pointer->Socket = value;
        }

        /// <summary>
        /// Reflect信息
        /// </summary>
        public WinDivertDataReflect Reflect
        {
            get => this.buffer.Pointer->Reflect;
            set => this.buffer.Pointer->Reflect = value;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.buffer.Clear();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.buffer.Dispose();
        }

        /// <summary>
        /// 隐式转换为SafeHandle
        /// </summary>
        /// <param name="addr"></param>
        public static implicit operator SafeHandle(WinDivertAddress addr)
        {
            return addr.buffer;
        }

        /// <summary>
        /// 缓存区
        /// </summary>
        private class WinDivertAddressBuffer : SafeHandleZeroOrMinusOneIsInvalid
        {
            public WinDivertAddressStruct* Pointer => (WinDivertAddressStruct*)this.handle.ToPointer();

            public WinDivertAddressBuffer()
                : base(true)
            {
                this.SetHandle(Marshal.AllocHGlobal(Size));
                this.Clear();
            }

            public void Clear()
            {
                var span = new Span<byte>(this.handle.ToPointer(), Size);
                span.Clear();
            }

            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(this.handle);
                this.SetHandle(IntPtr.Zero);
                return true;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct WinDivertAddressStruct
        {
            [FieldOffset(0)]
            public long Timestamp;

            [FieldOffset(8)]
            public WinDivertLayer Layer;

            [FieldOffset(9)]
            public WinDivertEvent Event;

            [FieldOffset(10)]
            public WinDivertAddressFlag Flags;

            [FieldOffset(11)]
            public byte Reserved;

            [FieldOffset(12)]
            public uint Reserved2;

            [FieldOffset(16)]
            public WinDivertDataNetwork Network;

            [FieldOffset(16)]
            public WinDivertDataFlow Flow;

            [FieldOffset(16)]
            public WinDivertDataSocket Socket;

            [FieldOffset(16)]
            public WinDivertDataReflect Reflect;

            [FieldOffset(16)]
            public fixed byte Reserved3[64];
        }
    }
}
