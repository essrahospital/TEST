using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class PowerTimerWindow : Form
    {
        private int RunCounter = 1;
        public static bool PowerTimeLoaded = false;
        public static int BeyondCounter = 0;
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public PowerTimerWindow()
        {
            InitializeComponent();
            FrmObj = this;
            this.Load += new System.EventHandler(this.LoadEvent);

            string powerFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");
            IniFile PowerIni = new IniFile(powerFile);
            System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(1000)).ContinueWith(task => GenPowerConfigView(PowerIni));
            PowerScheduleTimer.Enabled = true;
            PowerScheduleTimer.Start();
            PowerTimeLoaded = true;
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            form.ShowInTaskbar = true;
            form.WindowState = FormWindowState.Minimized;
        }

        private static void GenPowerConfigView(IniFile PowerIni)
        {
            var powerStatus = PowerIni.Read("Status", "System");
            if (powerStatus == "") { powerStatus = "Off"; }
            var powerType = PowerIni.Read("Type", "System");
            if (powerType == "") { powerType = "Virtual"; }

            // Get current Day and Time so we can compare to work out what values should be running
            DateTime now = DateTime.Now;
            var curDay = now.DayOfWeek.ToString();
            var curTime = now.ToString("HH:mm");
            string curOn = "";
            string curOff = "";
            string curOption = "";

            if (powerStatus == "On")
            {
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

                if (curDay == "Monday")
                {
                    curOn = monOn[0].ToString() + monOn[1].ToString() + ":" + monOn[2].ToString() + monOn[3].ToString();
                    curOff = monOff[0].ToString() + monOff[1].ToString() + ":" + monOff[2].ToString() + monOff[3].ToString();
                    curOption = monAllDay.ToString();
                }
                if (curDay == "Tuesday")
                {
                    curOn = tueOn[0].ToString() + tueOn[1].ToString() + ":" + tueOn[2].ToString() + tueOn[3].ToString();
                    curOff = tueOff[0].ToString() + tueOff[1].ToString() + ":" + tueOff[2].ToString() + tueOff[3].ToString();
                    curOption = tueAllDay.ToString();
                }
                if (curDay == "Wednesday")
                {
                    curOn = wedOn[0].ToString() + wedOn[1].ToString() + ":" + wedOn[2].ToString() + wedOn[3].ToString();
                    curOff = wedOff[0].ToString() + wedOff[1].ToString() + ":" + wedOff[2].ToString() + wedOff[3].ToString();
                    curOption = wedAllDay.ToString();
                }
                if (curDay == "Thursday")
                {
                    curOn = thuOn[0].ToString() + thuOn[1].ToString() + ":" + thuOn[2].ToString() + thuOn[3].ToString();
                    curOff = thuOff[0].ToString() + thuOff[1].ToString() + ":" + thuOff[2].ToString() + thuOff[3].ToString();
                    curOption = thuAllDay.ToString();
                }
                if (curDay == "Friday")
                {
                    curOn = friOn[0].ToString() + friOn[1].ToString() + ":" + friOn[2].ToString() + friOn[3].ToString();
                    curOff = friOff[0].ToString() + friOff[1].ToString() + ":" + friOff[2].ToString() + friOff[3].ToString();
                    curOption = friAllDay.ToString();
                }
                if (curDay == "Saturday")
                {
                    curOn = satOn[0].ToString() + satOn[1].ToString() + ":" + satOn[2].ToString() + satOn[3].ToString();
                    curOff = satOff[0].ToString() + satOff[1].ToString() + ":" + satOff[2].ToString() + satOff[3].ToString();
                    curOption = satAllDay.ToString();
                }
                if (curDay == "Sunday")
                {
                    curOn = sunOn[0].ToString() + sunOn[1].ToString() + ":" + sunOn[2].ToString() + sunOn[3].ToString();
                    curOff = sunOff[0].ToString() + sunOff[1].ToString() + ":" + sunOff[2].ToString() + sunOff[3].ToString();
                    curOption = sunAllDay.ToString();
                }

                if (curOption == "NA")
                {
                    FrmObj.label2.Text = "Screen On";
                    FrmObj.label2.Visible = true;
                    FrmObj.label3.Visible = true;
                    FrmObj.ScrOn.Text = curOn;
                    FrmObj.ScrOn.Visible = true;
                    FrmObj.ScrOff.Text = curOff;
                    FrmObj.ScrOff.Visible = true;
                }
                if (curOption == "On")
                {
                    FrmObj.label2.Text = "On All Day";
                    FrmObj.label2.Visible = true;
                    FrmObj.label3.Visible = false;
                    FrmObj.ScrOn.Text = "";
                    FrmObj.ScrOn.Visible = false;
                    FrmObj.ScrOff.Text = "";
                    FrmObj.ScrOff.Visible = false;
                }
                if (curOption == "Off")
                {
                    FrmObj.label2.Text = "Off All Day";
                    FrmObj.label2.Visible = true;
                    FrmObj.label3.Visible = false;
                    FrmObj.ScrOn.Text = "";
                    FrmObj.ScrOn.Visible = false;
                    FrmObj.ScrOff.Text = "";
                    FrmObj.ScrOff.Visible = false;
                }
            }
            else
            {
                FrmObj.label2.Text = "Not Active";
                FrmObj.label2.Visible = true;
                FrmObj.label3.Visible = false;
                FrmObj.ScrOn.Text = "";
                FrmObj.ScrOn.Visible = false;
                FrmObj.ScrOff.Text = "";
                FrmObj.ScrOff.Visible = false;
            }
            FrmObj.CurDay.Text = curDay;
            FrmObj.CurTime.Text = curTime;
            
        }

        static PowerTimerWindow _frmObj;
        public static PowerTimerWindow FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void PowerScheduleTimer_Tick(object sender, EventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            string powerFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");
            IniFile MyIni = new IniFile(configFile);
            IniFile PowerIni = new IniFile(powerFile);

            var maintMode = MyIni.Read("maintMode", "Network");
            var meLowPowerMode1 = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
            var meLowPowerMode2 = MyIni.Read("powersaveMode2", "Network");              // RS232 Low Power Mode

            var powerStatus = PowerIni.Read("Status", "System");
            if (powerStatus == "") { powerStatus = "Off"; }
            var powerType = PowerIni.Read("Type", "System");
            if (powerType == "") { powerType = "Virtual"; }

            if (powerStatus == "Off" && maintMode == "FALSE" && RunCounter == 1)
            {
                // if (powerType == "Virtual")
                // {
                    // if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONON", false, true); } else { GCMSSystem.TriggerSystem("MONON", true, false); }
                // }
                // if (powerType == "RS232")
                // {
                    // if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENON", false, true); } else { GCMSSystem.TriggerSystem("SCREENON", true, true); }
                // }
                // RunCounter++;
                // if (RunCounter == 50) { RunCounter = 1; }
            }
            if (powerStatus == "On" && maintMode == "FALSE")
            {
                // Debug.WriteLine("Power Status: On");
                // Debug.WriteLine("Maint Mode: Off");
                // Debug.WriteLine("-----------------------");
                GenPowerConfigView(PowerIni);
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
                var curTime = now.ToString("HH:mm");
                string curOn = "";
                string curOff = "";
                string curOption = "";

                // Debug.WriteLine("Cur Day: " + curDay);
                // Debug.WriteLine("Cur Time: " + curTime);
                // Debug.WriteLine("-----------------------");

                if (curDay == "Monday")
                {
                    curOn = monOn[0].ToString() + monOn[1].ToString() + ":" + monOn[2].ToString() + monOn[3].ToString();
                    curOff = monOff[0].ToString() + monOff[1].ToString() + ":" + monOff[2].ToString() + monOff[3].ToString();
                    curOption = monAllDay.ToString();
                }
                if (curDay == "Tuesday")
                {
                    curOn = tueOn[0].ToString() + tueOn[1].ToString() + ":" + tueOn[2].ToString() + tueOn[3].ToString();
                    curOff = tueOff[0].ToString() + tueOff[1].ToString() + ":" + tueOff[2].ToString() + tueOff[3].ToString();
                    curOption = tueAllDay.ToString();
                }
                if (curDay == "Wednesday")
                {
                    curOn = wedOn[0].ToString() + wedOn[1].ToString() + ":" + wedOn[2].ToString() + wedOn[3].ToString();
                    curOff = wedOff[0].ToString() + wedOff[1].ToString() + ":" + wedOff[2].ToString() + wedOff[3].ToString();
                    curOption = wedAllDay.ToString();
                }
                if (curDay == "Thursday")
                {
                    curOn = thuOn[0].ToString() + thuOn[1].ToString() + ":" + thuOn[2].ToString() + thuOn[3].ToString();
                    curOff = thuOff[0].ToString() + thuOff[1].ToString() + ":" + thuOff[2].ToString() + thuOff[3].ToString();
                    curOption = thuAllDay.ToString();
                }
                if (curDay == "Friday")
                {
                    curOn = friOn[0].ToString() + friOn[1].ToString() + ":" + friOn[2].ToString() + friOn[3].ToString();
                    curOff = friOff[0].ToString() + friOff[1].ToString() + ":" + friOff[2].ToString() + friOff[3].ToString();
                    curOption = friAllDay.ToString();
                }
                if (curDay == "Saturday")
                {
                    curOn = satOn[0].ToString() + satOn[1].ToString() + ":" + satOn[2].ToString() + satOn[3].ToString();
                    curOff = satOff[0].ToString() + satOff[1].ToString() + ":" + satOff[2].ToString() + satOff[3].ToString();
                    curOption = satAllDay.ToString();
                }
                if (curDay == "Sunday")
                {
                    curOn = sunOn[0].ToString() + sunOn[1].ToString() + ":" + sunOn[2].ToString() + sunOn[3].ToString();
                    curOff = sunOff[0].ToString() + sunOff[1].ToString() + ":" + sunOff[2].ToString() + sunOff[3].ToString();
                    curOption = sunAllDay.ToString();
                }

                TimeSpan now2 = DateTime.Now.TimeOfDay;
                TimeSpan start = TimeSpan.Parse(curOn);
                TimeSpan end = TimeSpan.Parse(curOff);
                TimeSpan startLimit = start.Add(TimeSpan.FromMinutes(5));
                TimeSpan endLimit = end.Add(TimeSpan.FromMinutes(5));
                RunCounter++;

                // Debug.WriteLine("Cur On: " + curOn);
                // Debug.WriteLine("Cur Off: " + curOff);
                // Debug.WriteLine("Cur Opt: " + curOption);
                // Debug.WriteLine("-----------------------");

                if (curOption == "NA")
                {
                    if (start <= end)
                    {
                        // start and stop times are in the same day
                        if (now2 >= start && now2 <= end)
                        {
                            // current time is between start and stop
                            // if (now2 >= start && now2 <= startLimit)
                            if (now2 >= start)
                            {
                                // Debug.WriteLine("current time is between start and stop & should turn ON - #1");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONON", false, true); } else { GCMSSystem.TriggerSystem("MONON", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENON", false, true); } else { GCMSSystem.TriggerSystem("SCREENON", true, true); }
                                }
                            }
                            if (now2 >= end)
                            {
                                // Debug.WriteLine("current time is between start and stop & should turn OFF - #2");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                                }
                            }
                        }
                        else
                        {
                            // Debug.WriteLine("current time is NOT between start and stop - #8");
                            if (now2 >= end && now2 <= endLimit)
                            {
                                // Debug.WriteLine("current time is between start and stop & should turn OFF - #3");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                                }
                            }
                            if (now2 >= end && BeyondCounter <= 30)
                            {
                                BeyondCounter++;
                                // Debug.WriteLine("current time is beyond stop & should turn OFF - #9");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                                }
                            }
                            if (BeyondCounter >= 100) { BeyondCounter = 0; }
                        }
                    }
                    else
                    {
                        // start and stop times are in different days
                        if (now2 >= start || now2 <= end)
                        {
                            // current time is between start and stop
                            if (now2 >= start && now2 <= startLimit)
                            {
                                // Debug.WriteLine("current time is between start and stop & should turn ON - #4");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONON", false, true); } else { GCMSSystem.TriggerSystem("MONON", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENON", false, true); } else { GCMSSystem.TriggerSystem("SCREENON", true, true); }
                                }
                            }
                            if (now2 >= end && now2 <= endLimit)
                            {
                                // Debug.WriteLine("current time is between start and stop & should turn OFF - #5");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                                }
                            }
                        }
                        else
                        {
                            // Debug.WriteLine("current time is NOT between start and stop - #6");
                            if (now2 >= end && now2 <= endLimit)
                            {
                                // Debug.WriteLine("current time is between start and stop & should turn OFF - #7");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                                }
                            }
                            if (now2 >= end && BeyondCounter <= 30)
                            {
                                BeyondCounter++;
                                // Debug.WriteLine("current time is beyond stop & should turn OFF - #10");
                                if (powerType == "Virtual")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                                }
                                if (powerType == "RS232")
                                {
                                    if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                                }
                            }
                            if (BeyondCounter >= 100) { BeyondCounter = 0; }
                        }
                    }
                }
                if (curOption == "On")
                {
                    // Screen On ALL DAY
                    if (now.Minute == 0 || now.Minute == 15 || now.Minute == 30 || now.Minute == 45)
                    {
                        // Debug.WriteLine("Screen On - ALL DAY");
                        // Its the Top of the hr, so we need to make sure we fire the commamnd to keep screen ON/OFF
                        if (powerType == "Virtual")
                        {
                            if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONON", false, true); } else { GCMSSystem.TriggerSystem("MONON", true, false); }
                        }
                        if (powerType == "RS232")
                        {
                            if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENON", false, true); } else { GCMSSystem.TriggerSystem("SCREENON", true, true); }
                        }
                    }
                }
                if (curOption == "Off")
                {
                    // Screen Off ALL DAY
                    if (now.Minute == 0 || now.Minute == 15 || now.Minute == 30 || now.Minute == 45)
                    {
                        // Debug.WriteLine("Screen Off - ALL DAY");
                        if (powerType == "Virtual")
                        {
                            if (MainForm.isDebug) { GCMSSystem.TriggerSystem("MONOFF", false, true); } else { GCMSSystem.TriggerSystem("MONOFF", true, false); }
                        }
                        if (powerType == "RS232")
                        {
                            if (MainForm.isDebug) { GCMSSystem.TriggerSystem("SCREENOFF", false, true); } else { GCMSSystem.TriggerSystem("SCREENOFF", true, true); }
                        }
                    }
                }

                if ((now.Hour == 0 && now.Minute == 0) && !MainForm.isUselessInternet)
                {
                    // Time is Midnight, so lets attempt to call home and make sure we have the correct Power Details
                    // This covers if the machine goes offline, after a power schedule update, this will mean it will update ASAP
                    if (MainForm.FrmObj.networkNameShort == "S")
                    {
                        GCMSSystem.TriggerSystem("UPDATEOFFLINEPWR", true, true);
                    }
                }
            }
            
        }
    }
}