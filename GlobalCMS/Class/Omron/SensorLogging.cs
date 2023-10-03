using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GlobalCMS
{
    public class Log
    {
        private Color[] color = new Color[4]
        {
      Color.White,
      Color.LightSkyBlue,
      Color.LightGreen,
      Color.LightPink
        };
        private string[] type_str = new string[4]
        {
      "System",
      "Tx",
      "Rx",
      "Error"
        };
        private RichTextBox richTextBox;
        private string csvPath;

        public Log(RichTextBox rtb, string fileHeader)
        {
            this.richTextBox = rtb;
            this.CSVHeader(fileHeader);
        }

        public void Write(Log.TYPE type, string message, bool disp, bool csv)
        {
            DateTime now = DateTime.Now;
            string str1 = now.ToString("yy/MM/dd HH:mm:ss");
            string str2 = now.ToString("HH:mm:ss:fff");
            try
            {
                if (disp)
                {
                    if (message.Trim() == "")
                        return;
                    string[] strArray = message.Split(new string[1]
                    {
            "\n"
                    }, StringSplitOptions.None);
                    this.richTextBox.Select(this.richTextBox.Text.Length, 0);
                    this.richTextBox.SelectedText = string.Empty;
                    this.richTextBox.SelectionFont = new Font(this.richTextBox.SelectionFont, FontStyle.Regular);
                    this.richTextBox.SelectionColor = this.color[(int)type];
                    this.richTextBox.AppendText(str2 + "\t" + strArray[0]);
                    for (int index = 1; index < strArray.Length; ++index)
                    {
                        this.richTextBox.AppendText(Environment.NewLine);
                        this.richTextBox.Select(this.richTextBox.Text.Length, 0);
                        this.richTextBox.SelectedText = string.Empty;
                        this.richTextBox.SelectionFont = new Font(this.richTextBox.SelectionFont, FontStyle.Regular);
                        this.richTextBox.SelectionColor = this.color[(int)type];
                        this.richTextBox.AppendText("\t\t" + strArray[index]);
                    }
                    this.richTextBox.AppendText(Environment.NewLine);
                    this.richTextBox.ScrollToCaret();
                    if (this.richTextBox.TextLength > 50000000)
                        this.richTextBox.Clear();
                    this.richTextBox.Update();
                }
                if (!csv)
                    return;
                StreamWriter streamWriter = new StreamWriter(this.csvPath, true, Encoding.GetEncoding("SHIFT-JIS"));
                streamWriter.Write(str1 + ",");
                streamWriter.Write(this.type_str[(int)type] + ",");
                message = message.Replace("\n", "\n,,");
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
            catch
            {
            }
        }

        public void NL()
        {
            this.richTextBox.Select(this.richTextBox.Text.Length, 0);
            this.richTextBox.SelectedText = string.Empty;
            this.richTextBox.AppendText(Environment.NewLine);
            this.richTextBox.ScrollToCaret();
            if (this.richTextBox.TextLength > 50000000)
                this.richTextBox.Clear();
            this.richTextBox.Update();
        }

        private void CSVHeader(string headString)
        {
            string path = Environment.CurrentDirectory + "\\logs\\envSensor";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            this.csvPath = Environment.CurrentDirectory + "\\logs\\envSensor\\" + DateTime.Now.ToString("yyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + "_" + headString + "_Log.csv";
            StreamWriter streamWriter = new StreamWriter(this.csvPath, true, Encoding.GetEncoding("utf-8"));
            streamWriter.Write("DateTime,");
            streamWriter.Write("Type,");
            streamWriter.Write(nameof(Log));
            streamWriter.Write("\r\n");
            streamWriter.Close();
        }

        public enum TYPE : byte
        {
            SYSTEM,
            TX,
            RX,
            ERROR,
        }
    }
}
