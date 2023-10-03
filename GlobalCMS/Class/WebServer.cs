using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace GlobalCMS
{
    class WebServer
    {
        public static TcpListener myListener;
        public static Thread th;
        public static bool isListening = true;
        static int globalPort = 300;

        public WebServer(int port)
        {
            try
            {
                //start listing on the given port
                myListener = new TcpListener(IPAddress.Any, port);
                myListener.Start();

                //start the thread which calls the method 'StartListen'
                th = new Thread(new ThreadStart(StartListen));
                th.Start();

                globalPort = port;          // Set the Global Port for the WebServer being run

            }
            catch (Exception)
            {
                // Console.WriteLine("An Exception Occurred while Listening :" + e.ToString());
            }
        }

        public void Stop()
        {
            isListening = false;
            try
            {
                myListener.Stop();
            }
            catch { }
            try
            {
                th.Abort();
            }
            catch { }
        }

        public static string GetTheDefaultFileName(string sLocalDirectory)
        {
            string sLine = "\\system.log";
            if (globalPort == 300)
            {
                sLine = "\\system.log";
            }
            else
            {
                sLine = "\\env.json";
            }
            return sLine;
        }
         
        public static string GetMimeType(string sRequestedFile)
        {
            StreamReader sr;
            string sLine = "";
            string sMimeType = "";
            string sFileExt = "";
            string sMimeExt = "";

            // Convert to lowercase
            sRequestedFile = sRequestedFile.ToLower();

            int iStartPos = sRequestedFile.IndexOf(".");

            sFileExt = sRequestedFile.Substring(iStartPos);

            try
            {
                //Open the Mime data file to find out the list of MIME Supported
                sr = new StreamReader("logs\\wwwMIME.www");
                while ((sLine = sr.ReadLine()) != null)
                {

                    sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        //find the separator
                        iStartPos = sLine.IndexOf(";");

                        // Convert to lower case
                        sLine = sLine.ToLower();

                        sMimeExt = sLine.Substring(0, iStartPos);
                        sMimeType = sLine.Substring(iStartPos + 1);

                        if (sMimeExt == sFileExt)
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // Console.WriteLine("An Exception Occurred : " + e.ToString());
            }

            if (sMimeExt == sFileExt)
                return sMimeType;
            else
                return "";
        }

        public static string GetLocalPath(string sMyWebServerRoot, string sDirName)
        {

            StreamReader sr;
            string sLine = "";
            string sVirtualDir = "";
            string sRealDir = "";
            int iStartPos = 0;

            //Remove extra spaces
            sDirName.Trim();

            // Convert to lowercase
            sMyWebServerRoot = sMyWebServerRoot.ToLower();

            // Convert to lowercase
            sDirName = sDirName.ToLower();

            //Remove the slash
            //sDirName = sDirName.Substring(1, sDirName.Length - 2);

            try
            {
                //Open the Vdirs.dat to find out the list virtual directories
                sr = new StreamReader("logs\\wwwVDirs.www");

                while ((sLine = sr.ReadLine()) != null)
                {
                    //Remove extra Spaces
                    sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        //find the separator
                        iStartPos = sLine.IndexOf(";");

                        // Convert to lowercase
                        sLine = sLine.ToLower();

                        sVirtualDir = sLine.Substring(0, iStartPos);
                        sRealDir = sLine.Substring(iStartPos + 1);

                        if (sVirtualDir == sDirName)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Console.WriteLine("An Exception Occurred : " + e.ToString());
            }

            if (sVirtualDir == sDirName)
            {
                return sRealDir;
            }
            else
            {
                return "";
            }
        }

        public static void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
        {
            string sBuffer = "";

            // if Mime type is not provided set default to text/html
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "text/plain";  // Default Mime Type is text/html
            }

            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Access-Control-Allow-Origin: *\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";

            byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);

            SendToBrowser2(bSendData, ref mySocket);

            // Console.WriteLine("Total Bytes : " + iTotBytes.ToString());

        }

        public static void SendToBrowser1(string sData, ref Socket mySocket)
        {
            SendToBrowser2(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }

        public static void SendToBrowser2(byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;

            try
            {
                if (mySocket.Connected)
                {
                    try
                    {
                        if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                        {
                            // Console.WriteLine("Socket Error cannot Send Packet");
                        }
                        else
                        {
                            // Console.WriteLine("No. of bytes send {0}", numBytes);
                        }
                    }
                    catch { }
                }
                else
                {
                    // Console.WriteLine("Connection Dropped....");
                }
            }
            catch (Exception)
            {
                // Console.WriteLine("Error Occurred : {0} ", e);
            }
        }

        //This method Accepts new connection and
        //First it receives the welcome massage from the client,
        //Then it sends the Current date time to the Client.
        public static async void StartListen()
        {
            int iStartPos = 0;
            string sRequest;
            string sDirName;
            string sRequestedFile;
            string sErrorMessage;
            string sLocalDir;
            string sMyWebServerRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs");
            string sPhysicalFilePath = "";
            // String sFormattedMessage = "";
            string sResponse = "";

            while (isListening)
            {
                //Accept a new connection
                try
                {
                    Socket mySocket = await myListener.AcceptSocketAsync();
                    // Console.WriteLine("Socket Type " + mySocket.SocketType);
                    if (mySocket.Connected)
                    {
                        // Console.WriteLine("\nClient Connected!!\n==================\nCLient IP {0}\n",  mySocket.RemoteEndPoint);

                        //make a byte array and receive data from the client 
                        byte[] bReceive = new byte[1024];
                        int i = mySocket.Receive(bReceive, bReceive.Length, 0);

                        //Convert Byte to String
                        string sBuffer = Encoding.ASCII.GetString(bReceive);

                        //At present we will only deal with GET type
                        if (sBuffer.Substring(0, 3) != "GET")
                        {
                            // Console.WriteLine("Only Get Method is supported..");
                        }

                        // Look for HTTP request
                        iStartPos = sBuffer.IndexOf("HTTP", 1);
                        string sHttpVersion = "HTTP/1.1";

                        try
                        {
                            // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                            sHttpVersion = sBuffer.Substring(iStartPos, 8);
                        }
                        catch { }

                        // Extract the Requested Type and Requested file/directory
                        try
                        {
                            sRequest = sBuffer.Substring(0, iStartPos - 1);

                            //Replace backslash with Forward Slash, if Any
                            sRequest.Replace("\\", "/");
                        }
                        catch
                        {
                            sRequest = "";
                        }

                        //If file name is not supplied add forward slash to indicate 
                        //that it is a directory and then we will look for the 
                        //default file name..
                        if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                        {
                            sRequest = sRequest + "/";
                        }


                        //Extract the requested file name
                        try
                        {
                            iStartPos = sRequest.LastIndexOf("/") + 1;
                        }
                        catch { }

                        if (globalPort == 300)
                        {
                            sRequestedFile = "system.log";
                        }
                        else
                        {
                            sRequestedFile = "env.json";
                        }
                        try
                        {
                            sRequestedFile = sRequest.Substring(iStartPos);
                        }
                        catch { }

                        //Extract The directory Name
                        sDirName = "/";
                        try
                        {
                            sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/") - 3);
                        }
                        catch { }


                        /////////////////////////////////////////////////////////////////////
                        // Identify the Physical Directory
                        /////////////////////////////////////////////////////////////////////
                        if (sDirName == "/")
                        {
                            sLocalDir = sMyWebServerRoot;
                        }
                        else
                        {
                            //Get the Virtual Directory
                            sLocalDir = GetLocalPath(sMyWebServerRoot, sDirName);
                        }

                        // Console.WriteLine("Directory Requested : " + sLocalDir);

                        //If the physical directory does not exists then
                        // dispaly the error message
                        if (sLocalDir.Length == 0)
                        {
                            sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
                            //sErrorMessage = sErrorMessage + "Please check data\\Vdirs.Dat";

                            //Format The Message
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);

                            //Send to the browser
                            SendToBrowser1(sErrorMessage, ref mySocket);

                            mySocket.Close();

                            continue;
                        }


                        /////////////////////////////////////////////////////////////////////
                        // Identify the File Name
                        /////////////////////////////////////////////////////////////////////

                        //If The file name is not supplied then look in the default file list
                        if (sRequestedFile.Length == 0)
                        {
                            // Get the default filename
                            sRequestedFile = GetTheDefaultFileName(sLocalDir);

                            if (sRequestedFile == "")
                            {
                                sErrorMessage = "<H2>Error!! No Default File Name Specified</H2>";
                                SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                                SendToBrowser1(sErrorMessage, ref mySocket);

                                mySocket.Close();

                                return;

                            }
                        }

                        /////////////////////////////////////////////////////////////////////
                        // Get TheMime Type
                        /////////////////////////////////////////////////////////////////////
                        string sMimeType = GetMimeType(sRequestedFile);

                        //Build the physical path
                        sPhysicalFilePath = sLocalDir + sRequestedFile;
                        // Console.WriteLine("File Requested : " + sPhysicalFilePath);

                        if (File.Exists(sPhysicalFilePath) == false)
                        {

                            sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser1(sErrorMessage, ref mySocket);

                            // Console.WriteLine(sFormattedMessage);
                        }

                        else
                        {
                            int iTotBytes = 0;

                            sResponse = "";

                            FileStream fs = new FileStream(sPhysicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            // Create a reader that can read bytes from the FileStream.


                            BinaryReader reader = new BinaryReader(fs);
                            byte[] bytes = new byte[fs.Length];
                            int read;
                            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                // Read from the file and write the data to the network
                                sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);

                                iTotBytes = iTotBytes + read;

                            }
                            reader.Close();
                            fs.Close();

                            SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                            SendToBrowser2(bytes, ref mySocket);
                            //mySocket.Send(bytes, bytes.Length,0);

                        }
                        mySocket.Close();
                    }
                }
                catch (ThreadAbortException) { }
                catch (Exception) { }
            }
        }
    }
}