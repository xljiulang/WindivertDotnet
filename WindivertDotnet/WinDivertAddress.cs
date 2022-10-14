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
    public unsafe class WinDivertAddress : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AddressStruct* Pointer => (AddressStruct*)this.handle.ToPointer();

        /// <summary>
        /// 获取WinDivertAddress结构大小
        /// </summary>
        public static int Size { get; } = sizeof(AddressStruct);

        /// <summary>
        /// 获取或设置发生的时间戳
        /// </summary>
        public long Timestamp
        {
            get => this.Pointer->Timestamp;
            set => this.Pointer->Timestamp = value;
        }

        /// <summary>
        /// 获取或设置所在层
        /// </summary>
        public WinDivertLayer Layer
        {
            get => this.Pointer->Layer;
            set => this.Pointer->Layer = value;
        }

        /// <summary>
        /// 获取或设置事件类型
        /// </summary>
        public WinDivertEvent Event
        {
            get => this.Pointer->Event;
            set => this.Pointer->Event = value;
        }

        /// <summary>
        /// 获取或设置标记
        /// </summary>
        public WinDivertAddressFlag Flags
        {
            get => this.Pointer->Flags;
            set => this.Pointer->Flags = value;
        }

        /// <summary>
        /// 获取或设置保留
        /// </summary>
        public byte Reserved
        {
            get => this.Pointer->Reserved;
            set => this.Pointer->Reserved = value;
        }

        /// <summary>
        /// 获取或设置保留
        /// </summary>
        public uint Reserved2
        {
            get => this.Pointer->Reserved2;
            set => this.Pointer->Reserved2 = value;
        }

        /// <summary>
        /// 获取网络信息的指针
        /// </summary>
        public WinDivertDataNetwork* Network => &this.Pointer->Network;

        /// <summary>
        /// 获取网络流信息的指针
        /// </summary>
        public WinDivertDataFlow* Flow => &this.Pointer->Flow;

        /// <summary>
        /// 获取套接字信息的指针
        /// </summary>
        public WinDivertDataSocket* Socket => &this.Pointer->Socket;

        /// <summary>
        /// 获取WinDivert信息的指针
        /// </summary>
        public WinDivertDataReflect* Reflect => &this.Pointer->Reflect;

        /// <summary>
        /// WinDivert地址信息
        /// </summary>
        public WinDivertAddress()
            : base(ownsHandle: true)
        {
            this.handle = MemoryNative.AllocZeroed(Size);
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            MemoryNative.Free(this.handle);
            return true;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            new Span<byte>(this.handle.ToPointer(), Size).Clear();
        }


        [StructLayout(LayoutKind.Explicit)]
        private struct AddressStruct
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
