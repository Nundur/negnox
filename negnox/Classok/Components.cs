using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static negnox.Classok.TCPHelper;
using static negnox.Program;
using static konzolmenuFejlesztes.konzolmenu;
using konzolmenuFejlesztes;

namespace negnox.Classok
{
    public static class Components
    {
        public static bool CheckTargetEnabled()
        {
            if (vanTarget && !string.IsNullOrEmpty(targetIp))
            {
                return true;
            }
            else
            {
                Log("[$Ez a parancs célpont specifikus!$]");
                return false;
            }
        }
        public static void LoadConfig()
        {
            string[] konfigelemek = File.ReadAllLines("config.nc");
            foreach (string beallitas in konfigelemek)
            {
                if (beallitas.Split(':')[0] == "showlogo") 
                    if (beallitas.Split(':')[1] == "0")
                    {

                        showLogo = 0;
                    }
                if (beallitas.Split(':')[1] == "1")
                {
                    showLogo = 1;

                }
                if (beallitas.Split(':')[1] == "2")
                {
                    showLogo = 2;

                }
                else if (beallitas.Split(':')[0] == "showpath") if (beallitas.Split(':')[1] == "false")
                    {
                        showPromptPath = false;
                    }
                    else showPromptPath = true;
                else if(beallitas.Split(':')[0] == "log")
                {
                    if (beallitas.Split(':')[1] == "false")
                    {
                        LogConsole = false;
                    }
                    else LogConsole = true;
                }
            }
        }
        public static void WritePrompt()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("negnox");
            if (showPromptPath)
            {
                Console.Write('@');
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Directory.GetCurrentDirectory());
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write('>');

        }
        public static void Kiiratas()
        {
            string negnoxTitle =
@"                                ████████    ██████   ███████ ████████    ██████  █████ █████
                               ▒▒███▒▒███  ███▒▒███ ███▒▒███▒▒███▒▒███  ███▒▒███▒▒███ ▒▒███ 
                                ▒███ ▒███ ▒███████ ▒███ ▒███ ▒███ ▒███ ▒███ ▒███ ▒▒▒█████▒  
                                ▒███ ▒███ ▒███▒▒▒  ▒███ ▒███ ▒███ ▒███ ▒███ ▒███  ███▒▒▒███ 
                                ████ █████▒▒██████ ▒▒███████ ████ █████▒▒██████  █████ █████
                               ▒▒▒▒ ▒▒▒▒▒  ▒▒▒▒▒▒   ▒▒▒▒▒███▒▒▒▒ ▒▒▒▒▒  ▒▒▒▒▒▒  ▒▒▒▒▒ ▒▒▒▒▒ 
                                                    ███ ▒███                                
                                                   ▒▒██████                                 
                                                    ▒▒▒▒▒▒";
            string negnoxTitle2 =
@"     ___ ___ ___ ___ ___ _ _ 
    |   | -_| . |   | . |_'_|
    |_|_|___|_  |_|_|___|_,_|
            |___|";
            
            string[] nTitleWSorok = negnoxTitle.Split('\n');
            //Console.WriteLine(negnoxTitle);
            //Console.WriteLine(negnoxTitle.Length);
            int szamlalo = 0;
            int maxSorok = nTitleWSorok.Length;
            //Console.WriteLine();
            //Console.WriteLine();

            
            if (showLogo == 1 )
            {
                Console.ForegroundColor = ConsoleColor.White;
                foreach (string sor in nTitleWSorok)
                {

                    if (szamlalo >= maxSorok / 5)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }

                    if (szamlalo > (maxSorok / 5) * 2 - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    if (szamlalo > (maxSorok / 5) * 3 - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (szamlalo > (maxSorok / 5) * 4 - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    if (szamlalo > (maxSorok / 5) * 5)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    Console.WriteLine(sor);

                    szamlalo++;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("                                 Network Enabled General Navigator & Operation Executioner");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("                                                          avagy");
                Console.WriteLine("                                       Nandi's Egyszerű Gép Navigátora és valami OX");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                Log($"                                                     version:÷{version}÷");
            } else if(showLogo == 2)
            {
                //Console.WriteLine("---------------------------------");
                Console.WriteLine(negnoxTitle2);
                //Console.WriteLine("---------------------------------");
                Console.WriteLine("Network Enabled General Navigator");
                Console.WriteLine("    & Operation Executioner");
                Log($"       version : ÷{version}÷");
                //Console.WriteLine("---------------------------------");
            }
            Console.WriteLine(  );

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
        public static string[] parameterDarabolas(string asd)
        {
            string[] returner = asd.Split('"').ToArray();
            return returner;
        }

        public static void MakeScript(string fileName)
        {
            if (File.Exists(fileName+".ns"))
            {
                Log($"$Már létezik ilyen script!$");
                return;
            }
            konzolmenu konzolmenu = new konzolmenuFejlesztes.konzolmenu();

            konzolmenu.KomplexAblak(3, 3, 95, 35, ConsoleColor.Black, ConsoleColor.Blue, true, ShadowType.faded, ConsoleColor.DarkGray, ConsoleColor.Black, "Egyszerű Szöveg Szerkesztő", ' ', true, TitleType.inBorder);
            konzolmenu.TextBlock("Mentes Es kilepes : ESC", 5, 4, ConsoleColor.DarkGray, ConsoleColor.Black);
            konzolmenu.TextBlock("---------------------------------------------------------------------------------------------", 4, 5, ConsoleColor.DarkCyan, ConsoleColor.Black);
            char[,] sorokcharba = konzolmenu.MTextBox(4, 6, 93, 31, ConsoleColor.Green, ConsoleColor.Black, ConsoleKey.Escape, true, false);

            string[] sorok = new string[sorokcharba.GetLength(0)];
            Console.Clear();

            for (int i = 0; i < sorokcharba.GetLength(0)-1; i++)
            {
                for (int k = 0; k < sorokcharba.GetLength(1)-1; k++)
                {
                    sorok[i] += sorokcharba[i, k];
                    //Console.Write($"{sorokcharba[i, k]}");
                }
                //Console.WriteLine();
            }

            foreach (string item in sorok)
            {
                Console.WriteLine(item);
            }
            File.WriteAllLines($".\\scripts\\{fileName}.ns", sorok);
            Console.ReadKey();


        }


        /// <summary>
        /// $ piros
        /// | zöld
        /// ÷ cyan
        /// </summary>
        /// 
        public static void Log(string message, ConsoleColor fg= ConsoleColor.Gray)// ennek a metodusnak az a lényege hogy ha | közé teszel valamit, zölden írja ki, ha $ közé teszel valamit, pirsos, ha pedig ÷ közé akkor cyan
        {
            LogTxt(message);
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

        public static void LogTxt(string message)
        {
            if (LogConsole) File.AppendAllText(".\\logs\\CL.txt", $"[{DateTime.Now}] => {message}\n");
        }
    }
}
