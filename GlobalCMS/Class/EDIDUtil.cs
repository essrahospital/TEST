using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WindowsDisplayAPI;

namespace EDIDPull
{
    static class EDIDUtil
    {
        #region Windows API stuff
        static Guid GUID_CLASS_MONITOR = new Guid(0x4d36e96e, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18);
        const int DIGCF_PRESENT = 0x00000002;
        const int ERROR_NO_MORE_ITEMS = 259;

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern uint RegEnumValue(
              UIntPtr hKey,
              uint dwIndex,
              StringBuilder lpValueName,
              ref uint lpcValueName,
              IntPtr lpReserved,
              IntPtr lpType,
              IntPtr lpData,
              ref int lpcbData);

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll")]
        internal static extern IntPtr SetupDiGetClassDevsEx(IntPtr ClassGuid,
            [MarshalAs(UnmanagedType.LPStr)]string enumerator,
            IntPtr hwndParent, int Flags, IntPtr DeviceInfoSet,
            [MarshalAs(UnmanagedType.LPStr)]string MachineName, IntPtr Reserved);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern int SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet,
            int MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

        [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern UIntPtr SetupDiOpenDevRegKey(
            IntPtr hDeviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            int scope,
            int hwProfile,
            int parameterRegistryValueKind,
            int samDesired);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(
            UIntPtr hKey);
        #endregion

        public static List<ScreenInformation> GetEDID()
        {
            List<ScreenInformation> lsi = new List<ScreenInformation>();
            IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
            Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
            IntPtr hDevInfo = SetupDiGetClassDevsEx(
                pGuid,
                null,
                IntPtr.Zero,
                DIGCF_PRESENT,
                IntPtr.Zero,
                null,
                IntPtr.Zero);

            DISPLAY_DEVICE dd = new DISPLAY_DEVICE
            {
                cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE))
            };
            uint dev = 0;

            bool bFoundDevice = false;
            while (EnumDisplayDevices(null, dev, ref dd, 0) && !bFoundDevice)
            {
                DISPLAY_DEVICE ddMon = new DISPLAY_DEVICE
                {
                    cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE))
                };
                uint devMon = 0;

                while (EnumDisplayDevices(dd.DeviceName, devMon, ref ddMon, 0) && !bFoundDevice)
                {
                    if ((ddMon.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) != 0 && (ddMon.StateFlags & DisplayDeviceStateFlags.MirroringDriver) == 0)
                    {
                        bFoundDevice = GetActualEDID(out string DeviceID, lsi);
                    }
                    devMon++;

                    ddMon = new DISPLAY_DEVICE
                    {
                        cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE))
                    };
                }

                dd = new DISPLAY_DEVICE
                {
                    cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE))
                };
                dev++;
            }

            return lsi;
        }

        const int DICS_FLAG_GLOBAL = 0x00000001;
        const int DIREG_DEV = 0x00000001;
        const int KEY_READ = 0x20019;
        private static bool GetActualEDID(out string DeviceID, List<ScreenInformation> lsi)
        {
            IntPtr pGuid = Marshal.AllocHGlobal(Marshal.SizeOf(GUID_CLASS_MONITOR));
            Marshal.StructureToPtr(GUID_CLASS_MONITOR, pGuid, false);
            IntPtr hDevInfo = SetupDiGetClassDevsEx(
                pGuid,
                null,
                IntPtr.Zero,
                DIGCF_PRESENT,
                IntPtr.Zero,
                null,
                IntPtr.Zero);

            DeviceID = string.Empty;

            if (null == hDevInfo)
            {
                Marshal.FreeHGlobal(pGuid);
                return false;
            }

            for (int i = 0; Marshal.GetLastWin32Error() != ERROR_NO_MORE_ITEMS; ++i)
            {
                SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA
                {
                    cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA))
                };

                if (SetupDiEnumDeviceInfo(hDevInfo, i, ref devInfoData) > 0)
                {
                    UIntPtr hDevRegKey = SetupDiOpenDevRegKey(
                        hDevInfo,
                        ref devInfoData,
                        DICS_FLAG_GLOBAL,
                        0,
                        DIREG_DEV,
                        KEY_READ);

                    if (hDevRegKey == null)
                        continue;

                    ScreenInformation si = PullEDID(hDevRegKey);
                    if (si != null)
                    {
                        lsi.Add(si);
                    }
                    RegCloseKey(hDevRegKey);
                }
            }

            Marshal.FreeHGlobal(pGuid);

            return true;
        }

        public const int ERROR_SUCCESS = 0;
        private static ScreenInformation PullEDID(UIntPtr hDevRegKey)
        {
            ScreenInformation si = null;
            StringBuilder valueName = new StringBuilder(128);
            uint ActualValueNameLength = 128;

            byte[] EDIdata = new byte[1024];
            IntPtr pEDIdata = Marshal.AllocHGlobal(EDIdata.Length);
            Marshal.Copy(EDIdata, 0, pEDIdata, EDIdata.Length);

            int size = 1024;
            for (uint i = 0, retValue = ERROR_SUCCESS; retValue != ERROR_NO_MORE_ITEMS; i++)
            {
                retValue = RegEnumValue(
                    hDevRegKey, i,
                    valueName, ref ActualValueNameLength,
                    IntPtr.Zero, IntPtr.Zero, pEDIdata, ref size); // EDIdata, pSize);

                string data = valueName.ToString();
                if (retValue != ERROR_SUCCESS || !data.Contains("EDID"))
                    continue;

                if (size < 1)                
                    continue;
                
                byte[] actualData = new byte[size];
                Marshal.Copy(pEDIdata, actualData, 0, size);                
                string hex = Encoding.ASCII.GetString(actualData);
                si = new ScreenInformation
                {
                    FullEDID = BitConverter.ToString(actualData),
                    Manufacturer = EDIDParse(actualData, "MODEL"),
                    Model = hex.Substring(90, 17).Trim().Replace("\0", string.Empty).Replace("?", string.Empty),
                    Part = hex.Substring(108, 17).Trim().Replace("\0", string.Empty).Replace("?", string.Empty),
                    Serial = EDIDParse(actualData, "SERIAL"),
                    Date = EDIDParse(actualData, "DATE"),
                    Year = EDIDParse(actualData, "YEAR"),
                    Size = EDIDParse(actualData, "SIZE"),
                    Digital = EDIDParse(actualData, "DIGITAL"),
                    Res = EDIDParse(actualData, "RES"),
                    Version = EDIDParse(actualData, "VERSION")
                };
            }

            Marshal.FreeHGlobal(pEDIdata);
            return si;
        }

        public static string EDIDParse(byte[] edidData, string whichVariable)
        {
            var theData = new EDIDParser.EDID(edidData);
            var theOut = "";

            if (whichVariable == "SIZE")
            {
                theOut = theData.DisplayParameters.DisplaySizeInInch.ToString();
            }
            if (whichVariable == "RES")
            {
                theOut = CmToPx(Convert.ToDouble(theData.DisplayParameters.PhysicalWidth.ToString())) + " x " + CmToPx(Convert.ToDouble(theData.DisplayParameters.PhysicalHeight.ToString()));
            }
            if (whichVariable == "DIGITAL")
            {
                theOut = theData.DisplayParameters.IsDigital.ToString();
            }
            if (whichVariable == "SERIAL")
            {
                theOut = theData.SerialNumber.ToString();
            }
            if (whichVariable == "MODEL")
            {
                theOut = theData.ManufacturerCode.ToString();
            }
            if (whichVariable == "YEAR")
            {
                theOut = theData.ManufactureYear.ToString();
            }
            if (whichVariable == "DATE")
            {
                theOut = theData.ManufactureDate.ToString();
            }
            if (whichVariable == "VERSION")
            {
                theOut = theData.EDIDVersion.ToString();
            }

            return theOut;
        }

        private struct PixelUnitFactor
        {
            public const double Px = 1;
            public const double Inch = 96;
            public const double Cm = 37.55;
            public const double Pt = 1;
        }

        public static double CmToPx(double cm)
        {
            return Math.Round(cm * PixelUnitFactor.Cm);
        }

        public static double PxToCm(double px)
        {
            return Math.Round(px / PixelUnitFactor.Cm);
        }
    }
}
