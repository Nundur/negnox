import negnox





def parancsDarabolas(asd):
    daraboltSz = asd.split(" ")
    daraboltSzoveg = []

    szamlalo = 0
    for szoveg in daraboltSz:
        szamlalo+=1
        if szamlalo == 1:
            continue
        else :
            daraboltSzoveg.append(szoveg)
    returner = ""
    szamlalo = 0
    for asd in daraboltSzoveg:
        szamlalo+=1
        returner+=asd
        
        if szamlalo != len(daraboltSzoveg):
            returner+=" "
    return returner
        
def parameterDarabolas(asd):
    returner = asd.split('"')
    return returner

def CheckTargetEnabled():
    #print(f"{negnox.Program.vanTarget}|{negnox.Program.targetIp}")
    print("")
    if not negnox.Program.targetIp == "":
        return True
    else :
        print("Ez a parancs célpont specifikus!!!")
        return False
def C(szin):
    if szin == "black":
        return "\x1b[90m"
    elif szin == "red":
        return "\x1b[91m"
    elif szin == "green":
        return "\x1b[92m"
    elif szin == "yellow":
        return "\x1b[93m"
    elif szin == "blue":
        return "\x1b[94m"
    elif szin == "magenta":
        return "\x1b[95m"
    elif szin == "cyan":
        return "\x1b[96m"
    elif szin == "white":
        return "\x1b[97m"
    else:
        return "\x1b[0m"
    
    
    
