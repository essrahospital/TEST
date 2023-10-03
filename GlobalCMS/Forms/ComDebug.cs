using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class ComDebug : Form
    {
        public ComDebug()
        {
            InitializeComponent();
            RunChecks();
        }
        
        private void RunChecks()
        {
            bool COM10 = SerialPort.GetPortNames().Any(x => x == "COM10");
            if (COM10)
            {
                COM10Label.Text = "Found";
                COM10Label.ForeColor = Color.FromArgb(0, 192, 0);
            }

            bool COM12 = SerialPort.GetPortNames().Any(x => x == "COM12");
            if (COM12)
            {
                COM12Label.Text = "Found";
                COM12Label.ForeColor = Color.FromArgb(0, 192, 0);
            }

            bool COM13 = SerialPort.GetPortNames().Any(x => x == "COM13");
            if (COM13)
            {
                COM13Label.Text = "Found";
                COM13Label.ForeColor = Color.FromArgb(0, 192, 0);
            }

            bool COM14 = SerialPort.GetPortNames().Any(x => x == "COM14");
            if (COM14)
            {
                COM14Label.Text = "Found";
                COM14Label.ForeColor = Color.FromArgb(0, 192, 0);
            }
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            RunChecks();
        }
    }
}
