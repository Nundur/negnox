





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