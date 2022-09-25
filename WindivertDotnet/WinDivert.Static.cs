using System;
using System.IO;

namespace WindivertDotnet
{
    partial class WinDivert
    {
        static WinDivert()
        {
            if (IntPtr.Size == 8)
            {
                WirteToAppData("WinDivert22/x64/WinDivert64.sys");
                WirteToAppData("WinDivert22/x64/WinDivert.dll", loadLibray: true);
            }
            else
            {
                WirteToAppData("WinDivert22/x86/WinDivert32.sys");
                WirteToAppData("WinDivert22/x86/WinDivert64.sys");
                WirteToAppData("WinDivert22/x86/WinDivert.dll", loadLibray: true);
            }
        }

        /// <summary>
        /// 将资源文件写入appData
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="loadLibray"></param>
        private static void WirteToAppData(string filePath, bool loadLibray = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var destFilePath = Path.Combine(appData, filePath);

            if (File.Exists(destFilePath) == false)
            {
                var dir = Path.GetDirectoryName(destFilePath);
                if (string.IsNullOrEmpty(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                var resoureName = $"{typeof(WinDivertNative).Namespace}.{filePath.Replace('/', '.')}";
                using var stream = typeof(WinDivert).Assembly.GetManifestResourceStream(resoureName);
                if (stream == null)
                {
                    throw new FileLoadException("Load resource file failure.", filePath);
                }

                using var fileStream = File.OpenWrite(destFilePath);
                stream.CopyTo(fileStream);
            }

            if (loadLibray == true)
            {
                Kernel32Native.LoadLibrary(destFilePath);
            }
        }
    }
}
