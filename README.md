# NEGNOX - Egy Post Exploitation Windows Controller 
Hello, és üdv, ez itt a negnox, egy admin-t az esetek szinte 0%-ban nem igénylő gép irányító program. Elődje volt a (szintén github fiókomon megtalálható) udphacker, ami nevéből fakadóan CSAK udp protokollt használt a kliensekkel való kommunikálásra, viszont az nem tudta normálisan kezelni a klienseket, ezért átírtam egy ilyen kisebb projekt-re.

*Rövid ismertetés*:
Maga a negnox jelentése a következő : Network Enabled General Navigator & Operation Executer
Az egész C# alapon működik, itt ott batch file is előfordul benne. A kapcsolatokat TCP kliensel (áldozat) és szerverrel (irányító) hozza létre, majd ezzel is kommunikál velük.

A program automatikusan logolja a legtöbb leütött parancsot és a kliensektől visszakapott értékeket, de ezt a funkciót, és még sok mást, ki lehet kapcsolni a konfigban (`config.nc`)

Személyre szabott payloadot (megadni hogy belső hálózaton melyik ipre csatlakozzon vissza) a build [ip] parancsal lehet csinálni például (`build 192.168.9.102`). Ez után elkészül a várva várt vírus a payload mappába ami vissza csatlakozik a megadott ip-re.

# Jelenleg futtatható használható parancsok:
<!--<img width="350"  alt="AAAAAAAdrgsfsrgrg" src="https://github.com/user-attachments/assets/9f2f40fd-1241-4c2b-a321-51af9936e788" /> -->
<img width="350" height="871" alt="commands" src="https://github.com/user-attachments/assets/f7eb6633-afe8-4a70-bb41-3f6498500bf6" />

# Egyéb megjegyzések :
A projekt használja egy másik projektemből 1 db C#-ban megírt class-t, a [konzolmenu](https://github.com/Nundur/konzolmenu)-t. Nem importáltam be az egész framework-ot, mert még nem volt rá szükség.
Mellesleg van egy harcoded jelszó benne a bejelentkezésnél, azt csak így eleinte tettem bele, még majd fejlesztem:)    (a psw : betmen27).

A későbbiekben még várhatóak frissítések, készülőben vannak már ezek :
- Negnox scripting language folytatása és fejlesztése (if parancs, while, és változók deklarálása).
- Negnox GUI wpf megoldással, és http listener kihasználásával
- Negnox scriptek futtatása paraméterekből (szintén http listenerrel)
- 
# képek:

<img width="300"  alt="starget" src="https://github.com/user-attachments/assets/c5260517-fddd-40c6-af09-f1385ac4ed46" />
<img width="300"  alt="msgbox" src="https://github.com/user-attachments/assets/a1d3a6e0-43e0-4e96-91c0-b3816da362b5" />
<img width="300"  alt="mkdir" src="https://github.com/user-attachments/assets/f3ca0849-b25f-451b-ba07-c112ce8f8738" />





<!--
<img width="300"  alt="negnoxlogomasik" src="https://github.com/user-attachments/assets/379a234a-3f9a-4d6e-9a5d-c7f3f73d922a" />
<img width="300"  alt="negnoxexample" src="https://github.com/user-attachments/assets/1e98f348-0352-4e70-89a0-5e2eae1210de" />
<img width="300"  alt="negnoxbejelentkezes" src="https://github.com/user-attachments/assets/d843122e-de1b-4963-b6d2-1fa16543ab11" />

-->

