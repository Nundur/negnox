using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security;
using System.Configuration;
using System.Xml.Schema;
using System.Threading;
using konzolmenuFejlesztes;
using System.Windows.Forms;
using System.Diagnostics;
using static negnox.Classok.Components;
using static negnox.Classok.TCPHelper;
using static negnox.Classok.Commandok;


namespace negnox
{
    public class Program
    {
        public static bool showPromptPath = true;
        public static bool isPromptLive = false;
        public static int showLogo = 1;

        public static string targetIp = "";
        public static bool vanTarget = false;

        public static string localExePath = Directory.GetCurrentDirectory();

        public static List<TcpClient> kliensek = new List<TcpClient>();
        public static string version = "";

        public static bool logSendingPackets = true;

        public static bool screenShotJon = false;


        public static Dictionary<string, string> variables = new Dictionary<string, string>();

        public static bool LogConsole = true;


        static async Task Main(string[] args)
        {

            Console.SetWindowSize(120, 50);
            Console.SetBufferSize(120, 9999);
            konzolmenu konzolmenu = new konzolmenu();

        passujra:

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            konzolmenu.KomplexAblak(39, 3, 42, 15, ConsoleColor.Gray, ConsoleColor.Black, true, ShadowType.faded, ConsoleColor.DarkGray, ConsoleColor.Black, "Log In", ' ', true, TitleType.inBorder);

            string pass = "betmen27";


            string[] logo = new string[] {
                "     ___ ___ ___ ___ ___ _ _",
                "    |   | -_| . |   | . |_'_|",
                "    |_|_|___|_  |_|_|___|_,_|",
                "            |___|",
                "Network Enabled General Navigator",
                "    & Operation Executioner"
            }; 
            konzolmenu.MTextBlock(logo, 44, 4, ConsoleColor.Black, ConsoleColor.Gray);

            konzolmenu.TextBlock("pass: ", 48, 12, ConsoleColor.Black, ConsoleColor.Gray);
            string userPass = konzolmenu.TextBox(54, 12, 18, ConsoleColor.Black, ConsoleColor.White, true);

            if (!(userPass == pass))
            {

                konzolmenu.KomplexAblak(50, 15, 21, 8, ConsoleColor.DarkRed, ConsoleColor.White, true, ShadowType.faded, ConsoleColor.DarkGray, ConsoleColor.Black, "Hibás Jelszó!", ' ', true, TitleType.inBorder);

                konzolmenu.Ablak(53, 17, 14, 3, ConsoleColor.Red, true, 2);
                konzolmenu.TextBlock("Próbáld újra", 54, 18, ConsoleColor.Black, ConsoleColor.Red);
                
                Console.ReadKey();
                goto passujra;
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();






            //config a fajlbol
            if (!File.Exists("config.nc"))
            {
                File.WriteAllText("config.nc", "showlogo:1\nshowpath:true\nversion:1.2.10");
            }
            try
            {
                LoadConfig();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (!File.Exists("target.n"))
            {
                File.WriteAllText("target.n", "");
            }

            version = File.ReadAllLines("config.nc")[2].Split(':')[1];
            //ablak meret beallitas
            Console.Title = "Negnox Console";
            //logo
            if(showLogo!=0) Kiiratas();
            //listener
            Task helo = listen();

            Thread.Sleep(1500);
            isPromptLive = true;
            //parancsok
            string x = "";
            while (true)
            {
                WritePrompt();
                x = Console.ReadLine();
                if (string.IsNullOrEmpty(x)) continue;
                isPromptLive = false;
                CheckCommand(x);
                isPromptLive = true;
            }

        }

    }
}
