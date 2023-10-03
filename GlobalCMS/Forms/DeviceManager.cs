using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;
using ListView = System.Windows.Forms.ListView;
using ListViewItem = System.Windows.Forms.ListViewItem;

namespace GlobalCMS
{
    public struct OSVERSIONINFO
    {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
    }

    public partial class DeviceManager : Form
    {
        public DeviceManager()
        {
            InitializeComponent();

            GetDrivers();

            var classes = DeviceClass.Load(DeviceFiter.AllClasses | DeviceFiter.Present);
            foreach (var cls in classes)
            {
                var classNode = treeView1.Nodes.Add(cls.Description);
                imageList1.Images.Add(cls.Icon);
                classNode.ImageIndex = imageList1.Images.Count - 1;
                classNode.SelectedImageIndex = classNode.ImageIndex;

                foreach (var device in cls.Devices)
                {
                    var deviceNode = classNode.Nodes.Add(device.Name);
                    imageList1.Images.Add(device.Icon);
                    deviceNode.ImageIndex = imageList1.Images.Count - 1;
                    deviceNode.SelectedImageIndex = deviceNode.ImageIndex;
                }
                // classNode.Expand();
            }

            // dispose (icons)
            foreach (var cls in classes)
            {
                foreach (var device in cls.Devices)
                {
                    device.Dispose();
                }
                cls.Dispose();
            }

            bool unoDriver = GCMSSystem.DriverSystem.CheckInstalled("SPSniff");
            if (unoDriver) { SensorBoardDriverResult.Text = "Installed"; }

            bool avaDriver = GCMSSystem.DriverSystem.CheckInstalled("usbser");
            if (avaDriver) { AVACameraDriverResult.Text = "Installed"; }

            bool envDriver = GCMSSystem.DriverSystem.CheckInstalled("FTDIBUS");
            if (envDriver) { ENVSensorDriverResult.Text = "Installed"; }

            bool nexusDriver = GCMSSystem.DriverSystem.CheckInstalled("Ser2pl");
            if (nexusDriver) { NexusSensorsDriverResult.Text = "Installed"; }
        }

        private void ExpandBTN_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
            CollapseBTN.Enabled = true;
            ExpandBTN.Enabled = false;
        }

        private void CollapseBTN_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
            CollapseBTN.Enabled = false;
            ExpandBTN.Enabled = true;
        }

        private void GetDrivers()
        {
            //Columns
            ColumnHeader nameColumn = new ColumnHeader();
            ColumnHeader pathColumn = new ColumnHeader();
            ColumnHeader descriptionColumn = new ColumnHeader();
            ColumnHeader statusColumn = new ColumnHeader();
            ColumnHeader startModeColumn = new ColumnHeader();
            //Declare, Search, and Get the Properties in Win32_SystemDriver
            System.Management.SelectQuery query = new System.Management.SelectQuery("Win32_SystemDriver");
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(query);
            foreach (System.Management.ManagementObject ManageObject in searcher.Get())
            {
                //Declare the Main Item
                ListViewItem item = new ListViewItem(ManageObject["Name"].ToString());
                //Create a Collection to hold all of the SubItems
                ListViewItem.ListViewSubItemCollection collection = new ListViewItem.ListViewSubItemCollection(item);
                //Declare All of the SubItems and Get the Appropriate Values
                ListViewItem.ListViewSubItem item2 = new ListViewItem.ListViewSubItem(item, ManageObject["PathName"].ToString());
                ListViewItem.ListViewSubItem item3 = new ListViewItem.ListViewSubItem(item, ManageObject["Description"].ToString());
                ListViewItem.ListViewSubItem item4 = new ListViewItem.ListViewSubItem(item, ManageObject["State"].ToString());
                ListViewItem.ListViewSubItem item5 = new ListViewItem.ListViewSubItem(item, ManageObject["StartMode"].ToString());
                //Add item2, item3, item4, and item5 to the Collection
                collection.Add(item2);
                collection.Add(item3);
                collection.Add(item4);
                collection.Add(item5);
                //Add item to the ListView
                listView1.Items.Add(item);
            }
            //Resize some of the Columns
            nameColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            statusColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            startModeColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        [DllImport("kernel32.Dll")]
        public static extern short GetVersionEx(ref OSVERSIONINFO o);
        static public string GetServicePack()
        {
            OSVERSIONINFO os = new OSVERSIONINFO();
            os.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFO));
            GetVersionEx(ref os);
            if (os.szCSDVersion == "")
                return "No Service Pack";
            else
                return os.szCSDVersion;
        }
    }
       
    public partial class WindowsAPI
    {
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int smIndex);

        public static bool IsWindowsActivated()
        {
            ManagementScope scope = new ManagementScope(@"\\" + System.Environment.MachineName + @"\root\cimv2");
            scope.Connect();

            SelectQuery searchQuery = new SelectQuery("SELECT * FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' and LicenseStatus = 1");
            ManagementObjectSearcher searcherObj = new ManagementObjectSearcher(scope, searchQuery);

            using (ManagementObjectCollection obj = searcherObj.Get())
            {
                return obj.Count > 0;
            }
        }

    }

    public class DeviceClass : IDisposable, IComparable, IComparable<DeviceClass>
    {
        private List<Device> _devices = new List<Device>();
        private Icon _icon;

        internal DeviceClass(Guid classId, string description)
        {
            ClassId = classId;
            Description = description;
        }

        public Guid ClassId { get; }
        public string Description { get; }
        public Icon Icon => _icon;
        public IReadOnlyList<Device> Devices => _devices;

        public static IReadOnlyList<DeviceClass> Load(DeviceFiter fiter)
        {
            var list = new List<DeviceClass>();
            var hdevinfo = SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, fiter);

            try
            {
                var data = new SP_DEVINFO_DATA();
                data.cbSize = Marshal.SizeOf<SP_DEVINFO_DATA>();
                int index = 0;
                while (SetupDiEnumDeviceInfo(hdevinfo, index, ref data))
                {
                    index++;
                    var classId = GetGuidProperty(hdevinfo, ref data, DEVPKEY_Device_ClassGuid);
                    if (classId == Guid.Empty)
                        continue;

                    string classDescription = GetClassDescription(classId);
                    var cls = list.FirstOrDefault(c => c.ClassId == classId);

                    if (classDescription == "Universal Serial Bus controllers" ||
                        classDescription == "Sound, video and game controllers" ||
                        classDescription == "Audio inputs and outputs" ||
                        classDescription == "Display adaptors" ||
                        classDescription == "Human Interface Devices" ||
                        classDescription == "Imaging devices" ||
                        classDescription == "Keyboards" ||
                        classDescription == "Mice and other pointing devices" ||
                        classDescription == "Monitors" ||
                        classDescription == "Network adapters" ||
                        classDescription == "Ports (COM & LPT)" ||
                        classDescription == "Processors" ||
                        classDescription == "Sound, video and game controllers" ||
                        classDescription == "Universal Serial Bus controllers")
                    {

                        if (cls == null)
                        {
                            cls = new DeviceClass(classId, classDescription);
                            list.Add(cls);

                            SetupDiLoadClassIcon(ref classId, out IntPtr clsIcon, out int mini);
                            if (clsIcon != IntPtr.Zero)
                            {
                                cls._icon = Icon.FromHandle(clsIcon);
                            }
                        }

                        string name = GetStringProperty(hdevinfo, ref data, DEVPKEY_Device_FriendlyName);
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            name = GetStringProperty(hdevinfo, ref data, DEVPKEY_Device_DeviceDesc);
                        }

                        Icon icon = null;
                        SetupDiLoadDeviceIcon(hdevinfo, ref data, 16, 16, 0, out IntPtr devIcon);
                        if (devIcon != IntPtr.Zero)
                        {
                            icon = Icon.FromHandle(devIcon);
                        }

                        var dev = new Device(cls, name, icon);
                        cls._devices.Add(dev);
                    }
                }

            }
            finally
            {
                if (hdevinfo != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(hdevinfo);
                }
            }

            foreach (var cls in list)
            {
                cls._devices.Sort();
            }
            list.Sort();
            return list;
        }

        int IComparable.CompareTo(object obj) => CompareTo(obj as DeviceClass);
        public int CompareTo(DeviceClass other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Description.CompareTo(other.Description);
        }

        public void Dispose()
        {
            if (_icon != null)
            {
                _icon.Dispose();
                _icon = null;
            }
        }

        private static string GetClassDescription(Guid classId)
        {
            SetupDiGetClassDescription(ref classId, IntPtr.Zero, 0, out int size);
            if (size == 0)
                return null;

            var ptr = Marshal.AllocCoTaskMem(size * 2);
            try
            {
                if (!SetupDiGetClassDescription(ref classId, ptr, size, out size))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return Marshal.PtrToStringUni(ptr, size - 1);
            }
            finally
            {
                Marshal.Release(ptr);
            }
        }

        private static string GetStringProperty(IntPtr hdevinfo, ref SP_DEVINFO_DATA data, DEVPROPKEY pk)
        {
            SetupDiGetDeviceProperty(hdevinfo, ref data, ref pk, out int propertyType, IntPtr.Zero, 0, out int size, 0);
            if (size == 0)
                return null;

            var ptr = Marshal.AllocCoTaskMem(size);
            try
            {
                if (!SetupDiGetDeviceProperty(hdevinfo, ref data, ref pk, out propertyType, ptr, size, out size, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return Marshal.PtrToStringUni(ptr, (size / 2) - 1);
            }
            finally
            {
                Marshal.Release(ptr);
            }
        }

        private static Guid GetGuidProperty(IntPtr hdevinfo, ref SP_DEVINFO_DATA data, DEVPROPKEY pk)
        {
            SetupDiGetDeviceProperty(hdevinfo, ref data, ref pk, out int propertyType, out Guid guid, 16, out int size, 0);
            return guid;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEVPROPKEY
        {
            public Guid fmtid;
            public int pid;
        }

        private const int ERROR_NOT_FOUND = 118;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        private static readonly DEVPROPKEY DEVPKEY_Device_DeviceDesc = new DEVPROPKEY { fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pid = 2 };
        private static readonly DEVPROPKEY DEVPKEY_Device_FriendlyName = new DEVPROPKEY { fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pid = 14 };
        private static readonly DEVPROPKEY DEVPKEY_Device_Class = new DEVPROPKEY { fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pid = 9 };
        private static readonly DEVPROPKEY DEVPKEY_Device_ClassGuid = new DEVPROPKEY { fmtid = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pid = 10 };

        [DllImport("setupapi", CharSet = CharSet.Unicode)]
        private static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, [MarshalAs(UnmanagedType.LPWStr)] string Enumerator, IntPtr hwndParent, DeviceFiter Flags);

        [DllImport("setupapi", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetupDiGetClassDescription(ref Guid ClassGuid, IntPtr ClassDescription, int ClassDescriptionSize, out int RequiredSize);

        [DllImport("setupapi", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetupDiLoadClassIcon(ref Guid ClassGuid, out IntPtr LargeIcon, out int MiniIconIndex);

        [DllImport("setupapi", SetLastError = true)]
        private static extern bool SetupDiLoadDeviceIcon(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData,
            int cxIcon, int cyIcon, int Flags, out IntPtr hIcon);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetupDiGetDeviceProperty(IntPtr DeviceInfoSet,
              ref SP_DEVINFO_DATA DeviceInfoData,
              ref DEVPROPKEY PropertyKey,
              out int PropertyType,
              IntPtr PropertyBuffer,
              int PropertyBufferSize,
              out int RequiredSize,
              int Flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetupDiGetDeviceProperty(IntPtr DeviceInfoSet,
              ref SP_DEVINFO_DATA DeviceInfoData,
              ref DEVPROPKEY PropertyKey,
              out int PropertyType,
              out Guid PropertyBuffer,
              int PropertyBufferSize,
              out int RequiredSize,
              int Flags);
    }

    [Flags]
    public enum DeviceFiter // DIGCF_* flags
    {
        Default = 1,
        Present = 2,
        AllClasses = 4,
        Profile = 8,
        DeviceInterface = 16
    }

    public class Device : IDisposable, IComparable, IComparable<Device>
    {
        internal Device(DeviceClass cls, string name, Icon icon)
        {
            Class = cls;
            Name = name;
            Icon = icon;
        }

        public string Name { get; }
        public DeviceClass Class { get; }
        public Icon Icon { get; private set; }

        public override string ToString() => Name;

        public void Dispose()
        {
            if (Icon != null)
            {
                Icon.Dispose();
                Icon = null;
            }
        }

        int IComparable.CompareTo(object obj) => CompareTo(obj as Device);
        public int CompareTo(Device other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Name.CompareTo(other.Name);
        }
    }
}