using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindivertDotnet
{
    static class Kernel32Native
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        /// <summary>
        /// Creates or opens a named or unnamed event object
        /// </summary>
        /// <param name="lpEventAttributes">
        /// A pointer to a SECURITY_ATTRIBUTES structure. If this parameter is NULL, the handle
        /// cannot be inherited by child processes.
        /// </param>
        /// <param name="bManualReset">
        /// If this parameter is TRUE, the function creates a manual-reset event object, which
        /// requires the use of the ResetEvent function to set the event state to nonsignaled. If
        /// this parameter is FALSE, the function creates an auto-reset event object, and system
        /// automatically resets the event state to nonsignaled after a single waiting thread has
        /// been released.
        /// </param>
        /// <param name="bInitialState">
        /// If this parameter is TRUE, the initial state of the event object is signaled; otherwise,
        /// it is nonsignaled.
        /// </param>
        /// <param name="lpName">
        /// The name of the event object. The name is limited to MAX_PATH characters. Name comparison
        /// is case sensitive.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the event object. If the named
        /// event object existed before the function call, the function returns a handle to the
        /// existing object and GetLastError returns ERROR_ALREADY_EXISTS.
        ///
        /// If the function fails, the return value is NULL. To get extended error information, call <seealso cref="Marshal.GetLastWin32Error" />.
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "CreateEvent", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, [MarshalAs(UnmanagedType.Bool)] bool bManualReset, [MarshalAs(UnmanagedType.Bool)] bool bInitialState, IntPtr lpName);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">
        /// A valid handle to an open object.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        ///
        /// If the function fails, the return value is zero.To get extended error information, call <seealso cref="Marshal.GetLastWin32Error" />.
        ///
        /// If the application is running under a debugger, the function will throw an exception if
        /// it receives either a handle value that is not valid or a pseudo-handle value. This can
        /// happen if you close a handle twice, or if you call CloseHandle on a handle returned by
        /// the FindFirstFile function instead of calling the FindClose function.
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Waits until the specified object is in the signaled state or the time-out interval elapses.
        /// </summary>
        /// <param name="hHandle">
        /// A handle to the object. For a list of the object types whose handles can be specified,
        /// see the following Remarks section.
        ///
        /// If this handle is closed while the wait is still pending, the function's behavior is undefined.
        /// </param>
        /// <param name="dwMilliseconds">
        /// The time-out interval, in milliseconds. If a nonzero value is specified, the function waits until the object is signaled or the interval elapses. If dwMilliseconds is zero, the function does not enter a wait state if the object is not signaled; it always returns immediately. If dwMilliseconds is INFINITE, the function will return only when the object is signaled.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value indicates the event that caused the function to return. It can be one of the following values.
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "WaitForSingleObject", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        /// <summary>
        /// Retrieves the results of an overlapped operation on the specified file, named pipe, or
        /// communications device.
        /// </summary>
        /// <param name="hFile">
        /// A handle to the file, named pipe, or communications device.
        /// </param>
        /// <param name="lpOverlapped">
        /// A pointer to an OVERLAPPED structure that was specified when the overlapped operation was started.
        /// </param>
        /// <param name="lpNumberOfBytesTransferred">
        /// A pointer to a variable that receives the number of bytes that were actually transferred by a read or write operation.
        /// </param>
        /// <param name="bWait">
        /// If this parameter is TRUE, and the Internal member of the lpOverlapped structure is
        /// STATUS_PENDING, the function does not return until the operation has been completed. If
        /// this parameter is FALSE and the operation is still pending, the function returns FALSE
        /// and the <seealso cref="Marshal.GetLastWin32Error" /> function returns ERROR_IO_INCOMPLETE.
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "GetOverlappedResult", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetOverlappedResult(IntPtr hFile, ref NativeOverlapped lpOverlapped, ref uint lpNumberOfBytesTransferred, [MarshalAs(UnmanagedType.Bool)] bool bWait);
    }
}
