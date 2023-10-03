using System;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GlobalCMS
{
    class MonitorControl
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public enum MonitorState
        {
            ON = -1,
            OFF = 2,
            STANDBY = 1
        }

        public static string Software(string whichTrigger, MonitorState state)
        {
            int SC_MONITORPOWER = 0xF170;
            uint WM_SYSCOMMAND = 0x0112;

            if (whichTrigger == "ON")
            {
                // Becuase of how weird Windows is not only do we need to set the POWERSTATE to ON
                // But we also need to trigger a mouse movement to get it out of MONOFF
                mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
                Thread.Sleep(40);
                mouse_event(MOUSEEVENTF_MOVE, 0, -1, 0, UIntPtr.Zero);
                SendKeys.SendWait("%");
            }

            if (whichTrigger == "OFF")
            {
                Form frm = new Form();
                SendMessage(frm.Handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)state);
                frm.Close();
            }

            return "Complete";
        }
    }
}