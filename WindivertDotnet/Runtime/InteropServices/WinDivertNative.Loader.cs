using System.IO;
using WindivertDotnet;

namespace System.Runtime.InteropServices
{
    static unsafe partial class WinDivertNative
    {
        static WinDivertNative()
        {
            if (Environment.Is64BitProcess)
            {
                LoadResource("WindivertDotnet.v222.x64.WinDivert64.sys");
                LoadResource("WindivertDotnet.v222.x64.WinDivert.dll", true);
            }
            else
            {
                LoadResource("WindivertDotnet.v222.x86.WinDivert32.sys");
                LoadResource("WindivertDotnet.v222.x86.WinDivert64.sys");
                LoadResource("WindivertDotnet.v222.x86.WinDivert.dll", true);
            }
        }

        /// <summary>
        /// º”‘ÿ◊ ‘¥
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loadLibray"></param>
        private static void LoadResource(string name, bool loadLibray = false)
        {
            var fileName = Path.GetFileNameWithoutExtension(name).Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(name);
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var destFilePath = Path.Combine(appDataPath, fileName);

            if (File.Exists(destFilePath) == false)
            {
                var dir = Path.GetDirectoryName(destFilePath);
                if (string.IsNullOrEmpty(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                using var rsStream = typeof(WinDivert).Assembly.GetManifestResourceStream(name)!;
                using var fileStream = File.OpenWrite(destFilePath);
                rsStream.CopyTo(fileStream);
            }

            if (loadLibray == true)
            {
                Kernel32Native.LoadLibrary(destFilePath);
            }
        }
    }
}
