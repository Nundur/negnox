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