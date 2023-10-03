using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GlobalCMS
{
    public static class TreeViewer
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        public static class helper
        {
            public static Process[] getChildProcesses(int parentProcessID)
            {
                var ret = new List<Process>();
                uint TH32CS_SNAPPROCESS = 2;

                IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                if (hSnapshot == IntPtr.Zero)
                {
                    return ret.ToArray();
                }
                PROCESSENTRY32 procInfo = new PROCESSENTRY32();
                procInfo.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
                if (Process32First(hSnapshot, ref procInfo) == false)
                {
                    return ret.ToArray();
                }
                do
                {
                    try
                    {
                        if ((int)procInfo.th32ParentProcessID == parentProcessID)
                        {
                            ret.Add(Process.GetProcessById((int)procInfo.th32ProcessID));
                        }
                    }
                    catch
                    {

                    }
                }
                while (Process32Next(hSnapshot, ref procInfo));
                return ret.ToArray();
            }
            public static void killChildProcesses(int parentProcessID)
            {
                foreach (var p in getChildProcesses(parentProcessID))
                {
                    try
                    {
                        p.Kill();
                    } 
                    catch
                    {

                    }
                }
            }
        }
    }
}