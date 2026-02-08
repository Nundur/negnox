import os
import TCPHelper
import threading
import time
import Commandok


def clear():
    os.system("cls" if os.name == "nt" else "clear")
def writeLogo():
    logo = [
    "     ___ ___ ___ ___ ___ _ _",
    "    |   | -_| . |   | . |_'_|",
    "    |_|_|___|_  |_|_|___|_,_|",
    "            |___|",
    "Network Enabled General Navigator",
    "    & Operation Executioner"]

    for f in logo:
        print(f)

def writePrompt():
    print("negnox>", end='')


class Program:
    showPromptPath = True
    isPromptLive = False
    showLogo = 1
    targetIp = ""
    vanTarget = False
    localExePath = os.getcwd()
    #public static List<TcpClient> kliensek = new List<TcpClient>();
    kliensek = []
    version = ""
    logSendingPackets = True
    screenShotJon = False
    #public static Dictionary<string, string> variables = new Dictionary<string, string>();
    LogConsole = True
    
    def main():
        
        writeLogo()
        psw = "betmen27"
        goodPsw = False
        print("Kérlek írd be a jelszót mielőtt belépsz:")
        #jelszo protect
        while not goodPsw :
            inputter = input(">")
            if inputter == psw:
                goodPsw = True
            else :
                print("Nem jó jelszo!")

        clear()
        writeLogo()
        
        Program.isPromptLive = True

        #listener    
        threading.Thread(
                target=TCPHelper.TCPHelper.listen,
                daemon=True
            ).start()
        time.sleep(1)
        


        while True:
            Program.isPromptLive = True
            x = input("negnox>")
            Program.isPromptLive = False
            Commandok.CheckCommand(x)
            







if __name__ == "__main__":
    Program.main()













