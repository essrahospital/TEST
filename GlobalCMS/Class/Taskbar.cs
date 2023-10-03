using System;
using System.Runtime.InteropServices;

namespace GlobalCMS
{
    class Taskbar
    {
        private const int SWP_HIDEWINDOW = 0x80;
        private const int SWP_SHOWWINDOW = 0x40;

        [DllImport("user32.dll")]

        public static extern bool SetWindowPos(
            int hWnd,                   //   handle to window    
            int hWndInsertAfter,        //   placement-order handle    
            short X,                    //   horizontal position    
            short Y,                    //   vertical position    
            short cx,                   //   width    
            short cy,                   //    height    
            uint uFlags                 //    window-positioning options    
        );

        [DllImport("user32.dll")]
        public static extern int FindWindow(
            string lpClassName,             //   class name    
            string lpWindowName             //   window name    
        );

        public static void Show()
        {
            int TaskBarHwnd;
            TaskBarHwnd = FindWindow("Shell_traywnd", "");
            SetWindowPos(TaskBarHwnd, 0, 0, 0, 0, 0, SWP_SHOWWINDOW);
        }

        public static void Hide()
        {
            int TaskBarHwnd;
            TaskBarHwnd = FindWindow("Shell_traywnd", "");
            SetWindowPos(TaskBarHwnd, 0, 0, 0, 0, 0, SWP_HIDEWINDOW);
        }

    }
    class TaskbarOLD
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int parentHandle, int childAfter, string className, int windowTitle);

        [DllImport("user32.dll")]
        private static extern int GetDesktopWindow();

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        protected static int Handle
        {
            get
            {
                return FindWindow("Shell_TrayWnd", "");
            }
        }

        protected static int HandleOfStartButton
        {
            get
            {
                int handleOfDesktop = GetDesktopWindow();
                int handleOfStartButton = FindWindowEx(handleOfDesktop, 0, "button", 0);
                return handleOfStartButton;
            }
        }

        public static void Show()
        {
            ShowWindow(Handle, SW_SHOW);
            ShowWindow(HandleOfStartButton, SW_SHOW);
        }

        public static void Hide()
        {
            ShowWindow(Handle, SW_HIDE);
            ShowWindow(HandleOfStartButton, SW_HIDE);
        }
    }
}
