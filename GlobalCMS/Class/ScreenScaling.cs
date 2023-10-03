using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GlobalCMS
{
    class ScreenScaling
    {
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117
        }

        public static float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            double ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            // 1 = 100%
            // 1.25 = 125%
            // 1.5 = 150%
            // 1.75 = 175%

            return (float)Math.Round(ScreenScalingFactor, 2);
        }

        public static void SetScaleFactor(string scaleFactor)
        {
            int ScaleFactorInt = 96;             // 96 is 100% (Default)
            if (scaleFactor == "125")
            {
                ScaleFactorInt = 120;
            }
            if (scaleFactor == "150")
            {
                ScaleFactorInt = 144;
            }
            if (scaleFactor == "200")
            {
                ScaleFactorInt = 192;
            }

            // This value should always exist in Windows 10 - Set to 1 to tell the system to use Custom Gfx Scaling
            try
            {
                var Win8DpiScalingCheck = GCMSSystem.Reg.CheckReg(Microsoft.Win32.RegistryHive.CurrentUser, "HKEY_CURRENT_USER\\Control Panel\\Desktop", "Win8DpiScaling");
                if (Win8DpiScalingCheck.ToString() == "0" && ScaleFactorInt != 96)
                {
                    try
                    {
                        GCMSSystem.Reg.UpdateReg(Microsoft.Win32.RegistryHive.CurrentUser, "HKEY_CURRENT_USER\\Control Panel\\Desktop", "Win8DpiScaling", 1);
                    }
                    catch { }
                }
            }
            catch { }

            // This value is only available once Custom Graphics Scaling is enabled - By Default this should be NULL as we will need to tell Windows to Enable Custom DPi Settings
            try
            {
                var LogPixelsCheck = GCMSSystem.Reg.CheckReg(Microsoft.Win32.RegistryHive.CurrentUser, "HKEY_CURRENT_USER\\Control Panel\\Desktop", "LogPixels");
                if (LogPixelsCheck == null)
                {
                    try
                    {
                        GCMSSystem.Reg.AddReg(Microsoft.Win32.RegistryHive.CurrentUser, "HKEY_CURRENT_USER\\Control Panel\\Desktop", "LogPixels");
                    }
                    catch { }
                }

                // once the 2 checks above have run then we can now run the Update on LogPixels to set the ScaleFactorInt
                try
                {
                    GCMSSystem.Reg.UpdateReg(Microsoft.Win32.RegistryHive.CurrentUser, "HKEY_CURRENT_USER\\Control Panel\\Desktop", "LogPixels", ScaleFactorInt);
                }
                catch { }
            }
            catch { }
        }
    }
}
