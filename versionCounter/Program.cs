using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace versionCounter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
            //Console.WriteLine(Directory.GetCurrentDirectory());

            string[] sorok = File.ReadAllLines("config.nc");

            string versionStr = sorok[2];

            int LastVersionNumber = Convert.ToInt32(versionStr.Split(':')[1].Split('.')[2]);

            string visszaIrni = $"version:{versionStr.Split(':')[1].Split('.')[0]}.{versionStr.Split(':')[1].Split('.')[1]}.{LastVersionNumber+1}";

            Console.WriteLine(visszaIrni);
            sorok[2] = visszaIrni;

            File.WriteAllLines("config.nc", sorok);
            Environment.Exit(0);

            //Console.ReadKey();
        }
    }
}
