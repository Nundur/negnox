using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace MCL
{
    internal class Program
    {

        // Egérműveletek
        [DllImport("user32.dll", SetLastError = true)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        // Egér események flag-jei
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;


        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        static string instruct = "getres";



        public static bool R = false;
        public static bool L = false;

        async static Task Main(string[] args)
        {

            //mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            //mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);


            int PORT = 9876;
            var udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            udp.EnableBroadcast = true;

            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data = Encoding.UTF8.GetBytes($"");
            await udp.SendAsync(data, data.Length, broadCastEP);
        asd:
            try
            {
                string elozox = "0";
                string elozoy = "0";
                string targetIp = "192.168.0.136";
                TcpClient client = new TcpClient(targetIp, 3232);

                UdpReceiveResult result;

                do
                {

                    //var stream = client.GetStream();
                    //byte[] buffer = new byte[1024];
                    //int bytes = stream.Read(buffer, 0, buffer.Length);
                    //string utasitas = Encoding.UTF8.GetString(buffer, 0, bytes);//itt kapja meg az üzeneteket
                    //string[] utasitasok = utasitas.Split('|');

                    result = await udp.ReceiveAsync();
                    byte[] message = result.Buffer;
                    string utasitas = Encoding.UTF8.GetString(message);

                    //Console.Write(utasitas);
                    //utasitas = utasitas.Split('|')[0];

                    //Thread.Sleep(20);
                    //Console.WriteLine($"--------------------------------{utasitas}-------------------------------");
                    
                    
                    //Console.WriteLine(utasitas);
                    if (utasitas == "off")
                    {
                        Environment.Exit(0);
                    }

                    Console.WriteLine(utasitas);
                    string[] command = utasitas.Split('|');
                    
                    if (elozox != command[0] || elozoy != command[1])
                    {
                        SetCursorPos(Convert.ToInt32(command[0]), Convert.ToInt32(command[1]));
                    }
                    
                    try
                    {
                        if (command[2] == "True")
                        {
                            if (!R)
                            {
                                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                                R = true;
                            }
                        } else
                        {
                            if (R)
                            {
                                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                                R = false;
                            }
                            
                        }
                        if (command[3] == "True")
                        {
                            if (!L)
                            {
                                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                                L = true;
                            }
                        } else
                        {
                            if (L)
                            {
                                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                                L = false;
                            }
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(e.ToString());
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    //x|y|Ron|Roff|Lon|Roff 

                    elozox = command[0];
                    elozoy = command[1];









                }
                while (udp.Available > 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                goto asd;
            }


            

        }
        
    }
}
