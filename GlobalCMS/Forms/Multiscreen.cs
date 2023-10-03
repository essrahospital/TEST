using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class Multiscreen : Form
    {
        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");      // Application Config

        public Multiscreen()
        {
            InitializeComponent();
            var MyIni = new IniFile(iniFile);
            var forceGFX_Monitor1 = MyIni.Read("Monitor1", "Display");
            if (forceGFX_Monitor1 == "") { MyIni.Write("Monitor1", "Disabled", "Display"); forceGFX_Monitor1 = "Landscape"; }

            var forceGFX_Monitor2 = MyIni.Read("Monitor2", "Display");
            if (forceGFX_Monitor2 == "") { MyIni.Write("Monitor2", "Disabled", "Display"); forceGFX_Monitor2 = "Disabled"; }

            var forceGFX_Monitor3 = MyIni.Read("Monitor3", "Display");
            if (forceGFX_Monitor3 == "") { MyIni.Write("Monitor3", "Disabled", "Display"); forceGFX_Monitor3 = "Disabled"; }

            var forceGFX_Monitor4 = MyIni.Read("Monitor4", "Display");
            if (forceGFX_Monitor4 == "") { MyIni.Write("Monitor4", "Disabled", "Display"); forceGFX_Monitor4 = "Disabled"; }

            var forceGFX_Monitor5 = MyIni.Read("Monitor5", "Display");
            if (forceGFX_Monitor5 == "") { MyIni.Write("Monitor5", "Disabled", "Display"); forceGFX_Monitor5 = "Disabled"; }

            var forceGFX_Monitor6 = MyIni.Read("Monitor6", "Display");
            if (forceGFX_Monitor6 == "") { MyIni.Write("Monitor6", "Disabled", "Display"); forceGFX_Monitor6 = "Disabled"; }

            // Reset the "Flipped" options for variables
            if (forceGFX_Monitor1 == "Landscape-Flip") { forceGFX_Monitor1 = "Landscape (Flipped)"; }
            if (forceGFX_Monitor1 == "Portrait-Flip") { forceGFX_Monitor1 = "Portrait (Flipped)"; }

            if (forceGFX_Monitor2 == "Landscape-Flip") { forceGFX_Monitor2 = "Landscape (Flipped)"; }
            if (forceGFX_Monitor2 == "Portrait-Flip") { forceGFX_Monitor2 = "Portrait (Flipped)"; }

            if (forceGFX_Monitor3 == "Landscape-Flip") { forceGFX_Monitor3 = "Landscape (Flipped)"; }
            if (forceGFX_Monitor3 == "Portrait-Flip") { forceGFX_Monitor3 = "Portrait (Flipped)"; }

            if (forceGFX_Monitor4 == "Landscape-Flip") { forceGFX_Monitor4 = "Landscape (Flipped)"; }
            if (forceGFX_Monitor4 == "Portrait-Flip") { forceGFX_Monitor4 = "Portrait (Flipped)"; }

            if (forceGFX_Monitor5 == "Landscape-Flip") { forceGFX_Monitor5 = "Landscape (Flipped)"; }
            if (forceGFX_Monitor5 == "Portrait-Flip") { forceGFX_Monitor5 = "Portrait (Flipped)"; }

            if (forceGFX_Monitor6 == "Landscape-Flip") { forceGFX_Monitor6 = "Landscape (Flipped)"; }
            if (forceGFX_Monitor6 == "Portrait-Flip") { forceGFX_Monitor6 = "Portrait (Flipped)"; }

            // Set the Dropdowns to their given options
            Monitor1.Text = forceGFX_Monitor1;
            Monitor2.Text = forceGFX_Monitor2;
            Monitor3.Text = forceGFX_Monitor3;
            Monitor4.Text = forceGFX_Monitor4;
            Monitor5.Text = forceGFX_Monitor5;
            Monitor6.Text = forceGFX_Monitor6;
        }

        private void UpdateBTN_Click(object sender, EventArgs e)
        {
            var MyIni = new IniFile(iniFile);

            var Monitor1_Value = Monitor1.Text;
            if (Monitor1_Value == "Landscape (Flipped)") { Monitor1_Value = "Landscape-Flip"; }
            if (Monitor1_Value == "Portrait (Flipped)") { Monitor1_Value = "Portrait-Flip"; }

            var Monitor2_Value = Monitor2.Text;
            if (Monitor2_Value == "Landscape (Flipped)") { Monitor2_Value = "Landscape-Flip"; }
            if (Monitor2_Value == "Portrait (Flipped)") { Monitor2_Value = "Portrait-Flip"; }

            var Monitor3_Value = Monitor3.Text;
            if (Monitor3_Value == "Landscape (Flipped)") { Monitor3_Value = "Landscape-Flip"; }
            if (Monitor3_Value == "Portrait (Flipped)") { Monitor3_Value = "Portrait-Flip"; }

            var Monitor4_Value = Monitor4.Text;
            if (Monitor4_Value == "Landscape (Flipped)") { Monitor4_Value = "Landscape-Flip"; }
            if (Monitor4_Value == "Portrait (Flipped)") { Monitor4_Value = "Portrait-Flip"; }

            var Monitor5_Value = Monitor5.Text;
            if (Monitor5_Value == "Landscape (Flipped)") { Monitor5_Value = "Landscape-Flip"; }
            if (Monitor5_Value == "Portrait (Flipped)") { Monitor5_Value = "Portrait-Flip"; }

            var Monitor6_Value = Monitor6.Text;
            if (Monitor6_Value == "Landscape (Flipped)") { Monitor6_Value = "Landscape-Flip"; }
            if (Monitor6_Value == "Portrait (Flipped)") { Monitor6_Value = "Portrait-Flip"; }

            MyIni.Write("Monitor1", Monitor1_Value, "Display");
            MyIni.Write("Monitor2", Monitor2_Value, "Display");
            MyIni.Write("Monitor3", Monitor3_Value, "Display");
            MyIni.Write("Monitor4", Monitor4_Value, "Display");
            MyIni.Write("Monitor5", Monitor5_Value, "Display");
            MyIni.Write("Monitor6", Monitor6_Value, "Display");

            if (Monitor1_Value != "Disabled")
            {
                if (Monitor1_Value == "Landscape")
                {
                    try { ScreenRotation.SetOrientation(1, 0); } catch { }
                }
                if (Monitor1_Value == "Landscape-Flip")
                {
                    try { ScreenRotation.SetOrientation(1, 90); } catch { }
                }
                if (Monitor1_Value == "Portrait")
                {
                    try { ScreenRotation.SetOrientation(1, 45); } catch { }
                }
                if (Monitor1_Value == "Portrait-Flip")
                {
                    try { ScreenRotation.SetOrientation(1, 135); } catch { }
                }
            }
            if (Monitor2_Value != "Disabled")
            {
                if (Monitor2_Value == "Landscape")
                {
                    try { ScreenRotation.SetOrientation(2, 0); } catch { }
                }
                if (Monitor2_Value == "Landscape-Flip")
                {
                    try { ScreenRotation.SetOrientation(2, 90); } catch { }
                }
                if (Monitor2_Value == "Portrait")
                {
                    try { ScreenRotation.SetOrientation(2, 45); } catch { }
                }
                if (Monitor2_Value == "Portrait-Flip")
                {
                    try { ScreenRotation.SetOrientation(2, 135); } catch { }
                }
            }
            if (Monitor3_Value != "Disabled")
            {
                if (Monitor3_Value == "Landscape")
                {
                    try { ScreenRotation.SetOrientation(3, 0); } catch { }
                }
                if (Monitor3_Value == "Landscape-Flip")
                {
                    try { ScreenRotation.SetOrientation(3, 90); } catch { }
                }
                if (Monitor3_Value == "Portrait")
                {
                    try { ScreenRotation.SetOrientation(3, 45); } catch { }
                }
                if (Monitor3_Value == "Portrait-Flip")
                {
                    try { ScreenRotation.SetOrientation(3, 135); } catch { }
                }
            }
            if (Monitor4_Value != "Disabled")
            {
                if (Monitor4_Value == "Landscape")
                {
                    try { ScreenRotation.SetOrientation(4, 0); } catch { }
                }
                if (Monitor4_Value == "Landscape-Flip")
                {
                    try { ScreenRotation.SetOrientation(4, 90); } catch { }
                }
                if (Monitor4_Value == "Portrait")
                {
                    try { ScreenRotation.SetOrientation(4, 45); } catch { }
                }
                if (Monitor4_Value == "Portrait-Flip")
                {
                    try { ScreenRotation.SetOrientation(4, 135); } catch { }
                }
            }
            if (Monitor5_Value != "Disabled")
            {
                if (Monitor5_Value == "Landscape")
                {
                    try { ScreenRotation.SetOrientation(5, 0); } catch { }
                }
                if (Monitor5_Value == "Landscape-Flip")
                {
                    try { ScreenRotation.SetOrientation(5, 90); } catch { }
                }
                if (Monitor5_Value == "Portrait")
                {
                    try { ScreenRotation.SetOrientation(5, 45); } catch { }
                }
                if (Monitor5_Value == "Portrait-Flip")
                {
                    try { ScreenRotation.SetOrientation(5, 135); } catch { }
                }
            }
            if (Monitor6_Value != "Disabled")
            {
                if (Monitor6_Value == "Landscape")
                {
                    try { ScreenRotation.SetOrientation(6, 0); } catch { }
                }
                if (Monitor6_Value == "Landscape-Flip")
                {
                    try { ScreenRotation.SetOrientation(6, 90); } catch { }
                }
                if (Monitor6_Value == "Portrait")
                {
                    try { ScreenRotation.SetOrientation(6, 45); } catch { }
                }
                if (Monitor6_Value == "Portrait-Flip")
                {
                    try { ScreenRotation.SetOrientation(6, 135); } catch { }
                }
            }
            MessageBox.Show("Multisceen Config Updated", "Multiscreen", MessageBoxButtons.OK);
        }

        private void FromServerBTN_Click(object sender, EventArgs e)
        {
            FromServerBTN.Enabled = false;
            UseWaitCursor = true;
            ControlBox = false;

            GCMSSystem.TriggerSystem("UPDATEMULTISCREEN", false, false);
            // Create the Dynamic Timer
            Timer tmr;
            tmr = new Timer();
            tmr.Tick += delegate
            {
                tmr.Stop();
                var MyIni = new IniFile(iniFile);
                var forceGFX_Monitor1 = MyIni.Read("Monitor1", "Display");
                if (forceGFX_Monitor1 == "") { MyIni.Write("Monitor1", "Disabled", "Display"); forceGFX_Monitor1 = "Landscape"; }

                var forceGFX_Monitor2 = MyIni.Read("Monitor2", "Display");
                if (forceGFX_Monitor2 == "") { MyIni.Write("Monitor2", "Disabled", "Display"); forceGFX_Monitor2 = "Disabled"; }

                var forceGFX_Monitor3 = MyIni.Read("Monitor3", "Display");
                if (forceGFX_Monitor3 == "") { MyIni.Write("Monitor3", "Disabled", "Display"); forceGFX_Monitor3 = "Disabled"; }

                var forceGFX_Monitor4 = MyIni.Read("Monitor4", "Display");
                if (forceGFX_Monitor4 == "") { MyIni.Write("Monitor4", "Disabled", "Display"); forceGFX_Monitor4 = "Disabled"; }

                var forceGFX_Monitor5 = MyIni.Read("Monitor5", "Display");
                if (forceGFX_Monitor5 == "") { MyIni.Write("Monitor5", "Disabled", "Display"); forceGFX_Monitor5 = "Disabled"; }

                var forceGFX_Monitor6 = MyIni.Read("Monitor6", "Display");
                if (forceGFX_Monitor6 == "") { MyIni.Write("Monitor6", "Disabled", "Display"); forceGFX_Monitor6 = "Disabled"; }

                // Reset the "Flipped" options for variables
                if (forceGFX_Monitor1 == "Landscape-Flip") { forceGFX_Monitor1 = "Landscape (Flipped)"; }
                if (forceGFX_Monitor1 == "Portrait-Flip") { forceGFX_Monitor1 = "Portrait (Flipped)"; }

                if (forceGFX_Monitor2 == "Landscape-Flip") { forceGFX_Monitor2 = "Landscape (Flipped)"; }
                if (forceGFX_Monitor2 == "Portrait-Flip") { forceGFX_Monitor2 = "Portrait (Flipped)"; }

                if (forceGFX_Monitor3 == "Landscape-Flip") { forceGFX_Monitor3 = "Landscape (Flipped)"; }
                if (forceGFX_Monitor3 == "Portrait-Flip") { forceGFX_Monitor3 = "Portrait (Flipped)"; }

                if (forceGFX_Monitor4 == "Landscape-Flip") { forceGFX_Monitor4 = "Landscape (Flipped)"; }
                if (forceGFX_Monitor4 == "Portrait-Flip") { forceGFX_Monitor4 = "Portrait (Flipped)"; }

                if (forceGFX_Monitor5 == "Landscape-Flip") { forceGFX_Monitor5 = "Landscape (Flipped)"; }
                if (forceGFX_Monitor5 == "Portrait-Flip") { forceGFX_Monitor5 = "Portrait (Flipped)"; }

                if (forceGFX_Monitor6 == "Landscape-Flip") { forceGFX_Monitor6 = "Landscape (Flipped)"; }
                if (forceGFX_Monitor6 == "Portrait-Flip") { forceGFX_Monitor6 = "Portrait (Flipped)"; }

                // Set the Dropdowns to their given options
                Monitor1.Text = forceGFX_Monitor1;
                Monitor2.Text = forceGFX_Monitor2;
                Monitor3.Text = forceGFX_Monitor3;
                Monitor4.Text = forceGFX_Monitor4;
                Monitor5.Text = forceGFX_Monitor5;
                Monitor6.Text = forceGFX_Monitor6;
                UpdateBTN.PerformClick();
                FromServerBTN.Enabled = true;
                UseWaitCursor = false;
                ControlBox = true;
            };

            // How Long do we want to run the Timer for
            tmr.Interval = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;

            // Start the Timer
            tmr.Start();
        }
    }
}
