using System;
using System.Linq;
using System.Management;

namespace GlobalCMS
{
    class Printer
    {
        public static uint PrintTestPage(string PrinterName, string MachineName)
        {
            ConnectionOptions connOptions = GetConnectionOptions();
            EnumerationOptions mOptions = GetEnumerationOptions(false);
            string machineName = string.IsNullOrEmpty(MachineName) ? Environment.MachineName : MachineName;
            ManagementScope mScope = new ManagementScope($@"\\{machineName}\root\CIMV2", connOptions);
            SelectQuery mQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            mQuery.QueryString += string.IsNullOrEmpty(PrinterName)
                                ? " WHERE Default = True"
                                : $" WHERE Name = '{PrinterName}'";
            mScope.Connect();

            using (ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(mScope, mQuery, mOptions))
            {
                ManagementObject moPrinter = moSearcher.Get().OfType<ManagementObject>().FirstOrDefault();
                if (moPrinter is null) throw new InvalidOperationException("Printer not found");

                InvokeMethodOptions moMethodOpt = new InvokeMethodOptions(null, ManagementOptions.InfiniteTimeout);
                using (ManagementBaseObject moParams = moPrinter.GetMethodParameters("PrintTestPage"))
                using (ManagementBaseObject moResult = moPrinter.InvokeMethod("PrintTestPage", moParams, moMethodOpt))
                    return (UInt32)moResult["ReturnValue"];
            }
        }

        private static EnumerationOptions GetEnumerationOptions(bool DeepScan)
        {
            EnumerationOptions mOptions = new EnumerationOptions()
            {
                Rewindable = false,        //Forward only query => no caching
                ReturnImmediately = true,  //Pseudo-async result
                DirectRead = true,         //Skip superclasses
                EnumerateDeep = DeepScan   //No recursion
            };
            return mOptions;
        }

        private static ConnectionOptions GetConnectionOptions()
        {
            ConnectionOptions connOptions = new ConnectionOptions()
            {
                EnablePrivileges = true,
                Timeout = ManagementOptions.InfiniteTimeout,
                Authentication = AuthenticationLevel.PacketPrivacy,
                Impersonation = ImpersonationLevel.Impersonate
            };
            return connOptions;
        }
    }
}
