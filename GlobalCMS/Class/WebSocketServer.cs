using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Drawing;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GlobalCMS
{
    class SocketServer
    {
        public static Thread th;
        public static TcpListener server;

        static SocketServer _frmObj;
        public static SocketServer FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        // Main Class and Event Trigger
        public SocketServer()
        {
            th = new Thread(new ThreadStart(StartListen));
            th.Start();

            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer = new System.Timers.Timer
            {
                Interval = 2000,
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += OnTimedEvent;
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            // This Timed event is for checking to make sure the Websocket Stays Open, incase of any sort of crash
            bool check = server.Server.IsBound;
            if (!check)
            {
                try { th.Abort(); } catch { }
                try { server.Server.Dispose(); } catch { }
                try
                {
                    th = new Thread(new ThreadStart(StartListen));
                    th.Start();
                }
                catch { }
            }
        }

        // Class Functions
        public void Stop()
        {
            server.Stop();
            th.Abort();
            
        }
        public void Restart()
        {
            server.Stop();
            th.Abort();

            th = new Thread(new ThreadStart(StartListen));
            th.Start();
        }
        
        // public void StartListen()
        public static async void StartListen()
        {
            int port = 2525;
            server = new TcpListener(IPAddress.Any, port);
            try
            {
                server.Server.ReceiveTimeout = 5;
                server.Server.SendTimeout = 5;
                try
                {
                    server.Start();
                }
                catch { }


                // TcpClient client = server.AcceptTcpClient();
                TcpClient client = await server.AcceptTcpClientAsync();
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    // enter to an infinite cycle to be able to handle every change in stream
                    while (true)
                    {
                        while (!stream.DataAvailable) ;
                        while (client.Available < 3) ;

                        byte[] bytes = new byte[client.Available];
                        // Debug.WriteLine("Bytes Data: " + bytes.ToString());
                        stream.Read(bytes, 0, client.Available);
                        string s = Encoding.UTF8.GetString(bytes);
                        // Debug.WriteLine("Raw Data: " + s.ToString());
                        if (s == "GCMSEndInteractive")
                        {
                            // Debug.WriteLine("Detected GCMSEndInteractive Websocket to end X-Frame Interactive");
                            try
                            {
                                MainForm.FrmObj.LastWebsocketLabel.Text = s;
                            }
                            catch { }

                            try
                            {
                                SystemDebug.FrmObj.WebsocketBox.AppendText("\r" + DateTime.Now.ToString("dd MMM HH:mm:ss") + " - " + s);
                                SystemDebug.FrmObj.WebsocketBox.ScrollToCaret();
                            }
                            catch { }
                            EndInteractiveXFrame();
                            break;
                        }
                        else
                        {
                            if (Regex.IsMatch(s, "^POST", RegexOptions.IgnoreCase))
                            {
                                // Debug.WriteLine("Detected _POST Data");
                                string outJsonInner = s.Split(new char[] { '{', '}' })[1];
                                string outJSON = "{" + outJsonInner + "}";
                                // Debug.WriteLine("outJSON: " + outJSON.ToString());
                                var JSON = (JObject)JsonConvert.DeserializeObject(outJSON);
                                string theCMD = JSON["command"].Value<string>();
                                // Debug.WriteLine("theCMD: " + theCMD);
                                try
                                {
                                    MainForm.FrmObj.LastWebsocketLabel.Text = theCMD.ToUpper();
                                }
                                catch { }

                                try
                                {
                                    SystemDebug.FrmObj.WebsocketBox.AppendText("\r" + DateTime.Now.ToString("dd MMM HH:mm:ss") + " - " + theCMD.ToUpper());
                                    SystemDebug.FrmObj.WebsocketBox.ScrollToCaret();
                                }
                                catch { }

                                try
                                {
                                    GCMSSystem.TriggerSystem(theCMD.ToUpper(), false, true);
                                }
                                catch { }
                                break;
                            }
                            else if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase))
                            {
                                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                                // 3. Compute SHA-1 and Base64 hash of the new value
                                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                                string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                                string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                                byte[] swkaSha1 = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                                string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                                // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                                byte[] response = Encoding.UTF8.GetBytes(
                                    "HTTP/1.1 101 Switching Protocols\r\n" +
                                    "Connection: Upgrade\r\n" +
                                    "Upgrade: websocket\r\n" +
                                    "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                                try
                                {
                                    stream.Write(response, 0, response.Length);
                                }
                                catch { }
                                break;
                            }
                            else
                            {
                                // must be true, "All messages from the client to the server have this bit set"
                                bool fin = (bytes[0] & 0b10000000) != 0, mask = (bytes[1] & 0b10000000) != 0;
                                int opcode = bytes[0] & 0b00001111, msglen = bytes[1] - 128, offset = 2;

                                if (msglen == 126)
                                {
                                    msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                                    offset = 4;
                                }
                                else if (msglen == 127)
                                {
                                    // i don't really know the byte order, possibly requires editing in future
                                }

                                if (msglen == 0) { }
                                else if (mask)
                                {
                                    byte[] decoded = new byte[msglen];
                                    byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                                    offset += 4;

                                    for (int i = 0; i < msglen; ++i)
                                    {
                                        decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);
                                    }
                                    string text = Encoding.UTF8.GetString(decoded).Trim();

                                    string asAscii = Encoding.ASCII.GetString(
                                        Encoding.Convert(
                                            Encoding.UTF8,
                                            Encoding.GetEncoding(
                                                Encoding.ASCII.EncodingName,
                                                new EncoderReplacementFallback(string.Empty),
                                                new DecoderExceptionFallback()
                                                ),
                                            Encoding.UTF8.GetBytes(text)
                                        )
                                    );
                                    asAscii = RemoveSpecialCharacters(asAscii);

                                    if (asAscii != "")
                                    {
                                        try
                                        {
                                            MainForm.FrmObj.LastWebsocketLabel.Text = asAscii;
                                        }
                                        catch { }

                                        try
                                        {
                                            SystemDebug.FrmObj.WebsocketBox.AppendText("\r" + DateTime.Now.ToString("dd MMM HH:mm:ss") + " - " + asAscii);
                                            SystemDebug.FrmObj.WebsocketBox.ScrollToCaret();
                                        }
                                        catch { }

                                        try
                                        {
                                            GCMSSystem.TriggerSystem(asAscii.ToUpper(), false, true);
                                        }
                                        catch { }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    try { client.Close(); } catch { }
                    try { server.Stop(); } catch { }
                }
            }
            catch { }
        }
        private static void EndInteractiveXFrame()
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var myKeyboard = MyIni.Read("Load", "Keyboard");

            if (MainForm.isInteractive)
            {
                var curWebView = "Error";
                try
                {
                    curWebView = SignageBrowser.FrmObj.wBrowser.WebView.Url;
                }
                catch { }

                MainForm.isInteractive = false;      // Set to False to Stop the Timer Checker

                if (!MainForm.isInLockdown)
                {
                    // Reset the Mode Back to Normal
                    MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                    MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                }
                // Stop the Interactive Timer and Restart the Checker for Interactive
                if (MainForm.isAutoCookieCleaner)
                {
                    GCMSSystem.Chrome.ClearCookies(true);
                }

                if (myKeyboard == "Application")
                {
                    // Make sure that the Onscreen Keyboard is Closed (if being used)
                    GCMSSystem.OSK.StopOSK();
                    // if we are using the Application OSK then we also need to clear the last position of the keyboard as this is a full reset
                    // Open up the XML Document ready to modify the location
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "keyboard", "Layouts", "Default.xml"));
                    XmlElement documentElement = xmlDocument.DocumentElement;

                    // Set the New Attributes in the XML
                    documentElement.SetAttribute("top", "0");
                    documentElement.SetAttribute("left", "0");

                    // Save the Updates into the XML File
                    xmlDocument.Save(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "keyboard", "Layouts", "Default.xml"));
                }

                // Send The End Interactive Websocket - "endinteractive"
                GCMSSystem.NodeSocket.Send("endinteractive");
                GCMSSystem.InteractiveLog.Send("End");

                MainForm.FrmObj.Interaction.Stop();
                int interval = MainForm.FrmObj.Interaction.Interval;
                MainForm.FrmObj.Interaction.Interval = interval;
                MainForm.FrmObj.Interaction.Start();

                if (curWebView != "Error" && curWebView != "http://127.0.0.1:444/" && curWebView != "https://127.0.0.1:444/")
                {
                    // Detected that the Kiosk has moved outside of 127.0.0.1 so we need to trigger the function for forwarding back to 127.0.0.1
                    var MySystemLoad = MyIni.Read("Load", "Browser");
                    var MySystemKeyboard = MyIni.Read("Keyboard", "Browser");
                    var BrowserSSL = MyIni.Read("SSL", "Browser");

                    if (MySystemLoad == "Default")
                    {
                        if (SignageBrowser.isXFrame)
                        {
                            // This means that it has had to leave our system for Printing Support and other Elements outside of a iFrame
                            if (BrowserSSL == "On")
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "https://127.0.0.1:444";
                            }
                            else
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "http://127.0.0.1:444";
                            }
                            SignageBrowser.isXFrame = false;        // Reset for Next Use
                        }
                        else
                        {
                            if (BrowserSSL == "On")
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=on";
                            }
                            else
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=off";
                            }
                        }
                    }
                }
            }
            MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
            MainForm.FrmObj.CheckForInteractive.Start();
        }
    }
}