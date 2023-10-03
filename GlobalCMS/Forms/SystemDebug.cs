using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class SystemDebug : Form
    {
        public SystemDebug()
        {
            InitializeComponent();
        }

        private void SignageBrowserDebug_Load(object sender, EventArgs e)
        {
            FrmObj = this;
            GenStatus();
            GenOfflinePwrSettings();
        }

        private static void GenOfflinePwrSettings()
        {
            string powerFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");
            IniFile PowerIni = new IniFile(powerFile);
            var powerStatus = PowerIni.Read("Status", "System");
            if (powerStatus == "On") { FrmObj.OfflinePwrStatus.Text = "Active"; FrmObj.OfflinePwrStatus.ForeColor = Color.FromArgb(0, 192, 0); }
            if (powerStatus == "" || powerStatus == "Off") { FrmObj.OfflinePwrStatus.Text = "Not Active"; FrmObj.OfflinePwrStatus.ForeColor = Color.FromArgb(192, 0, 0); }

            if (powerStatus == "Off")
            {
                // If powerStatus is turned off then we need to hide the Power On and Power Off Labels
                FrmObj.OfflinePwrOn.Visible = false;
                FrmObj.OfflinePwrOnLabel.Visible = false;
                FrmObj.OfflinePwrOff.Visible = false;
                FrmObj.OfflinePwrOffLabel.Visible = false;
            }
            if (powerStatus == "On")
            {
                // if the powerStatus is turned on - Then explode all the data so we can work out which day we are looking for
                var powerScheduleMon = PowerIni.Read("Mon", "Schedule");
                var powerScheduleTue = PowerIni.Read("Tue", "Schedule");
                var powerScheduleWed = PowerIni.Read("Wed", "Schedule");
                var powerScheduleThu = PowerIni.Read("Thu", "Schedule");
                var powerScheduleFri = PowerIni.Read("Fri", "Schedule");
                var powerScheduleSat = PowerIni.Read("Sat", "Schedule");
                var powerScheduleSun = PowerIni.Read("Sun", "Schedule");

                // Split the String into an Array for On and Off
                var monStr = powerScheduleMon.Split(',');
                var monOn = monStr[0];
                var monOff = monStr[1];
                var monAllDay = monStr[2];

                var tueStr = powerScheduleTue.Split(',');
                var tueOn = tueStr[0];
                var tueOff = tueStr[1];
                var tueAllDay = tueStr[2];

                var wedStr = powerScheduleWed.Split(',');
                var wedOn = wedStr[0];
                var wedOff = wedStr[1];
                var wedAllDay = wedStr[2];

                var thuStr = powerScheduleThu.Split(',');
                var thuOn = thuStr[0];
                var thuOff = thuStr[1];
                var thuAllDay = thuStr[2];

                var friStr = powerScheduleFri.Split(',');
                var friOn = friStr[0];
                var friOff = friStr[1];
                var friAllDay = friStr[2];

                var satStr = powerScheduleSat.Split(',');
                var satOn = satStr[0];
                var satOff = satStr[1];
                var satAllDay = satStr[2];

                var sunStr = powerScheduleSun.Split(',');
                var sunOn = sunStr[0];
                var sunOff = sunStr[1];
                var sunAllDay = sunStr[2];

                // Get current Day and Time so we can compare to work out what values should be running
                DateTime now = DateTime.Now;
                var curDay = now.DayOfWeek.ToString();
                string curOn = "";
                string curOff = "";

                if (curDay == "Monday")
                {
                    curOn = monOn[0].ToString() + monOn[1].ToString() + ":" + monOn[2].ToString() + monOn[3].ToString();
                    curOff = monOff[0].ToString() + monOff[1].ToString() + ":" + monOff[2].ToString() + monOff[3].ToString();
                }
                if (curDay == "Tuesday")
                {
                    curOn = tueOn[0].ToString() + tueOn[1].ToString() + ":" + tueOn[2].ToString() + tueOn[3].ToString();
                    curOff = tueOff[0].ToString() + tueOff[1].ToString() + ":" + tueOff[2].ToString() + tueOff[3].ToString();
                }
                if (curDay == "Wednesday")
                {
                    curOn = wedOn[0].ToString() + wedOn[1].ToString() + ":" + wedOn[2].ToString() + wedOn[3].ToString();
                    curOff = wedOff[0].ToString() + wedOff[1].ToString() + ":" + wedOff[2].ToString() + wedOff[3].ToString();
                }
                if (curDay == "Thursday")
                {
                    curOn = thuOn[0].ToString() + thuOn[1].ToString() + ":" + thuOn[2].ToString() + thuOn[3].ToString();
                    curOff = thuOff[0].ToString() + thuOff[1].ToString() + ":" + thuOff[2].ToString() + thuOff[3].ToString();
                }
                if (curDay == "Friday")
                {
                    curOn = friOn[0].ToString() + friOn[1].ToString() + ":" + friOn[2].ToString() + friOn[3].ToString();
                    curOff = friOff[0].ToString() + friOff[1].ToString() + ":" + friOff[2].ToString() + friOff[3].ToString();
                }
                if (curDay == "Saturday")
                {
                    curOn = satOn[0].ToString() + satOn[1].ToString() + ":" + satOn[2].ToString() + satOn[3].ToString();
                    curOff = satOff[0].ToString() + satOff[1].ToString() + ":" + satOff[2].ToString() + satOff[3].ToString();
                }
                if (curDay == "Sunday")
                {
                    curOn = sunOn[0].ToString() + sunOn[1].ToString() + ":" + sunOn[2].ToString() + sunOn[3].ToString();
                    curOff = sunOff[0].ToString() + sunOff[1].ToString() + ":" + sunOff[2].ToString() + sunOff[3].ToString();
                }

                FrmObj.OfflinePwrOn.Text = curOn;
                FrmObj.OfflinePwrOff.Text = curOff;

                FrmObj.OfflinePwrOn.Visible = true;
                FrmObj.OfflinePwrOnLabel.Visible = true;
                FrmObj.OfflinePwrOff.Visible = true;
                FrmObj.OfflinePwrOffLabel.Visible = true;
            }
        }

        private static void GenStatus()
        {
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(@"http://localhost:444/status");
                var jsonData = JsonConvert.DeserializeObject<dynamic>(json);
                FrmObj.JSONBox.Text = jsonData.ToString();
            }
        }

        private HttpWebRequest GetRequest(string url, string httpMethod = "GET", bool allowAutoRedirect = true)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

            request.Timeout = Convert.ToInt32(new TimeSpan(0, 5, 0).TotalMilliseconds);
            request.Method = httpMethod;
            return request;
        }

        static SystemDebug _frmObj;
        public static SystemDebug FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void RefreshBTN_Click(object sender, EventArgs e)
        {
            GenStatus();
        }

        private void ClearHistoryBTN_Click(object sender, EventArgs e)
        {
            WebsocketBox.Clear();
            WebsocketBox.Focus();
        }
    }
}
