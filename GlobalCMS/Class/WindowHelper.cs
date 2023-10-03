using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace GlobalCMS
{
    public static class WindowHelper
    {
        private const int SW_RESTORE = 9;
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static void BringToFront(string title)
        {
            try
            {
                if (!string.IsNullOrEmpty(title))
                {
                    IEnumerable<IntPtr> listPtr = null;

                    // Wait until the browser is started - it may take some time
                    // Maximum wait is (200 + some) * 100 milliseconds > 20 seconds
                    int retryCount = 100;
                    do
                    {
                        listPtr = FindWindowsWithText(title);
                        if (listPtr == null || listPtr.Count() == 0)
                        {
                            Thread.Sleep(200);
                        }
                    } while (--retryCount > 0 || listPtr == null || listPtr.Count() == 0);

                    if (listPtr == null)
                        return;

                    foreach (var hWnd in listPtr)
                    {
                        if (IsIconic(hWnd))
                            ShowWindow(hWnd, SW_RESTORE);
                        SetForegroundWindow(hWnd);
                    }
                }
            }
            catch (Exception)
            {
                // If it fails at least we tried
            }
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size++ > 0)
            {
                var builder = new StringBuilder(size);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return string.Empty;
        }

        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (GetWindowText(wnd).Contains(titleText))
                {
                    windows.Add(wnd);
                }
                return true;
            }, IntPtr.Zero);

            return windows;
        }
    }
}
