using System;
using System.Windows.Forms;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GlobalCMS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            Application.ThreadException += GlobalThreadExceptionHandler;

            string[] args = Environment.GetCommandLineArgs();

            foreach (string arg in args) {
                if (arg.Trim().ToLower() == "disabledefender" || arg.Trim().ToLower() == "enabledefender")
                {
                    // GlobalCMS.GCMSSystem.DefenderTrigger(arg);
                    return;
                }
            }

            Process mobj_pro = System.Diagnostics.Process.GetCurrentProcess();
            Process[] mobj_proList = System.Diagnostics.Process.GetProcessesByName(mobj_pro.ProcessName);
            if (mobj_proList.Length > 1)
            {
                // MessageBox.Show("!!Error!!\nOnly one instance of this program can be run", "GlobalCMS Monitoring Solution", MessageBoxButtons.OK);
                return;
            }

            string user = Environment.CommandLine.Split(' ').Last();
            Environment.SetEnvironmentVariable("LOCALAPPDATA", Path.Combine("C:\\", "Users", user, "AppData", "Local"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            if (File.Exists(iniFile))
            {
                Application.Run(new SplashScreen());
            }
            else
            {
                Application.Run(new Commission());
            }
        }

        [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]
        class UIHostNoLaunch
        {
        }

        [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ITipInvocation
        {
            void Toggle(IntPtr hwnd);
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            //Environment.Exit(1);
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            //Environment.Exit(1);
        }
    }
}
