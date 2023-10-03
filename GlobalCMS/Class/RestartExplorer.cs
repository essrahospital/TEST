using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GlobalCMS
{
    class RestartExplorer
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const int WM_USER = 0x0400;

        public static void Restart()
        {
            try
            {
                var ptr = FindWindow("Shell_TrayWnd", null);
                PostMessage(ptr, WM_USER + 436, (IntPtr)0, (IntPtr)0);
                int i = 0;

                do
                {
                    ptr = FindWindow("Shell_TrayWnd", null);
                    if (ptr.ToInt32() == 0)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                } while (true && i <= 10);
            } catch { }

            string explorer = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
            Process process = new Process();
            process.StartInfo.FileName = explorer;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }
}
