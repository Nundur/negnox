using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace konzolmenuFejlesztes
{
    enum Orientation { horizontal, vertical }
    enum MenuType { Boring, Window, WindowWithShadow }
    enum ShadowType { block, faded }

    enum TitleType { inBorder, inWindow }
    class konzolmenu
    {
        /// <summary>
        /// ablak(x y szélesség, hosszúság, színek, árnyék,arnyek szin)
        /// Árnyék szin : 1 = fekete, 2 = Sötétszürke
        /// </summary>
        public void Ablak(int x, int y, int szelesseg, int hosszusag, ConsoleColor szinek, bool arnyek, int arnyekszin)
        {

            //▒
            if (arnyekszin == 1) { Console.ForegroundColor = ConsoleColor.Black; Console.BackgroundColor = ConsoleColor.DarkGray; }
            else { Console.ForegroundColor = ConsoleColor.DarkGray; Console.BackgroundColor = ConsoleColor.Black; }
            Console.SetCursorPosition(x + 1, y + 1);
            if (arnyek)
            {
                for (int i = 0; i < hosszusag; i++)
                {
                    
                    Console.SetCursorPosition(x + 1, y + 1 + i);
                    for (int k = 0; k < szelesseg; k++)
                    {
                        Console.Write("▒");
                    }
                    //Line('█', hosszusag, x + szelesseg, y + 1, Orientation.vertical, ConsoleColor.Black, ConsoleColor.DarkGray);
                    //Line('█', szelesseg, x + 1, y + hosszusag, Orientation.horizontal, ConsoleColor.Black, ConsoleColor.DarkGray);
                    //▀
                }

            }
            //█
            Console.ForegroundColor = szinek;
            Console.BackgroundColor = szinek;
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < hosszusag; i++)
            {
                Console.SetCursorPosition(x, y + i);
                for (int k = 0; k < szelesseg; k++)
                {
                    Console.Write("█");
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        //✅️
        public void KomplexAblak(int x, int y, int szelesseg, int hosszusag, ConsoleColor hatterSzin, ConsoleColor betuSzin, bool arnyek, ShadowType shadowtype, ConsoleColor arnyekSzin, ConsoleColor arnyekHatterSzin, string cim, char cimVonal, bool szegely, TitleType TitleTypee)
        {
            //║╗╝═╔╚
            //▒
            Console.ForegroundColor = arnyekSzin;
            Console.BackgroundColor = arnyekHatterSzin;
            Console.SetCursorPosition(x + 1, y + 1);
            if (arnyek)
            {
                if (shadowtype == ShadowType.faded)
                {
                    Line('▒', hosszusag, x + szelesseg, y + 1, Orientation.vertical, arnyekSzin, arnyekHatterSzin);
                    Line('▒', szelesseg, x + 1, y + hosszusag, Orientation.horizontal, arnyekSzin, arnyekHatterSzin);
                }
                else if (shadowtype == ShadowType.block)
                {
                    Line('█', hosszusag, x + szelesseg, y + 1, Orientation.vertical, arnyekSzin, arnyekHatterSzin);
                    Line('▀', szelesseg, x + 1, y + hosszusag, Orientation.horizontal, arnyekSzin, arnyekHatterSzin);
                }
            }
            //█
            Console.ForegroundColor = hatterSzin;
            Console.BackgroundColor = hatterSzin;

            Console.SetCursorPosition(x, y);
            for (int i = 0; i < hosszusag; i++)
            {
                Console.SetCursorPosition(x, y + i);
                for (int k = 0; k < szelesseg; k++)
                {
                    Console.Write("█");
                }
            }
            //cim
            Console.ForegroundColor = betuSzin;


            //cimVonal
            Console.SetCursorPosition(x, y + 2);
            for (int i = 0; i < szelesseg; i++)
            {
                Console.Write(cimVonal);
            }

            //szegely
            if (szegely)
            {
                Console.SetCursorPosition(x, y);
                for (int k = 0; k < szelesseg; k++)
                {

                    if (k == 0)
                    {
                        Console.Write("╔");
                    }
                    else if (k == szelesseg - 1)
                    {
                        Console.Write("╗");
                    }
                    else
                    {
                        Console.Write("═");
                    }


                }
                Console.SetCursorPosition(x, y + hosszusag - 1);
                for (int k = 0; k < szelesseg; k++)
                {
                    if (k == 0)
                    {
                        Console.Write("╚");
                    }
                    else if (k == szelesseg - 1)
                    {
                        Console.Write("╝");
                    }
                    else
                    {
                        Console.Write("═");
                    }
                }

                for (int i = 1; i < hosszusag - 1; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write("║");
                    Console.SetCursorPosition(x + szelesseg - 1, y + i);
                    Console.Write("║");
                }

            }
            if (TitleTypee == TitleType.inWindow)
            {
                Console.ForegroundColor = betuSzin;
                Console.BackgroundColor = hatterSzin;
                int hovaCim = szelesseg / 2 - cim.Length / 2;
                Console.SetCursorPosition(x + hovaCim, y + 1);
                Console.Write(cim);
            }
            else
            {
                Console.ForegroundColor = betuSzin;
                Console.BackgroundColor = hatterSzin;
                int hovaCim = szelesseg / 2 - cim.Length / 2;
                Console.SetCursorPosition(x + hovaCim, y);
                Console.Write(cim);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// MenuPontok(tomb, x, y, terkoz, milyen szinu, kivalasztottSZin, foregroundcolor)
        /// </summary>
        public int Buttons(MenuType MenuStilusa, string[] elemek, int x, int y, int tavolsag, ConsoleColor szin, ConsoleColor kivalasztottSzin, ConsoleColor betuSzin)
        {
            int eredetix = x;
            int eredetiy = y;
            int kimenet = 1;
            int kivalasztott = 1;
            Console.ForegroundColor = betuSzin;
            Console.BackgroundColor = szin;
            bool igaze = true;
            switch (MenuStilusa)
            {
                case MenuType.Boring:
                    while (igaze)
                    {
                        Console.SetCursorPosition(x, y);
                        for (int i = 0; i < elemek.Length; i++)
                        {
                            Console.SetCursorPosition(x, y + i);
                            if (kivalasztott - 1 == i) Console.BackgroundColor = kivalasztottSzin;
                            Console.Write(elemek[i]);

                            y += tavolsag;

                            Console.BackgroundColor = szin;
                        }
                        //billentyuvárás
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.DownArrow:
                                if (kivalasztott + 1 <= elemek.Length)
                                {
                                    kivalasztott++;
                                    kimenet++;
                                }
                                break;
                            case ConsoleKey.UpArrow:
                                if (kivalasztott - 1 > 0)
                                {
                                    kivalasztott--;
                                    kimenet--;
                                }
                                break;
                            case ConsoleKey.Enter:
                                igaze = false;
                                break;
                        }
                        x = eredetix;
                        y = eredetiy;
                    }
                    break;
                case MenuType.Window:
                    while (igaze)
                    {
                        Console.SetCursorPosition(x, y);
                        for (int i = 0; i < elemek.Length; i++)
                        {
                            Console.SetCursorPosition(x, y + i);
                            if (kivalasztott - 1 == i) Console.BackgroundColor = kivalasztottSzin;
                            Console.Write(elemek[i]);

                            y += tavolsag;

                            Console.BackgroundColor = szin;
                        }
                        //billentyuvárás
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.DownArrow:
                                if (kivalasztott + 1 <= elemek.Length)
                                {
                                    kivalasztott++;
                                    kimenet++;
                                }
                                break;
                            case ConsoleKey.UpArrow:
                                if (kivalasztott - 1 > 0)
                                {
                                    kivalasztott--;
                                    kimenet--;
                                }
                                break;
                            case ConsoleKey.Enter:
                                igaze = false;
                                break;
                        }
                        x = eredetix;
                        y = eredetiy;
                    }

                    break;
                case MenuType.WindowWithShadow:
                    break;
                default:
                    break;
            }



            return kimenet;
        }


        /// <summary>
        /// MenuLista(tomb, x, y, szelesseg, hosszusag, foregoundcolor, backgroundcolor, menucolor, selectedMenuColor, orientation (amugy vertical))
        /// </summary>
        /// //✅️
        public int MenuLista(string[] elemek, int x, int y, int szelesseg, int hosszusag, ConsoleColor ForeGround, ConsoleColor BackGround, ConsoleColor Menu, ConsoleColor SelectedMenu, Orientation merre = Orientation.vertical, bool arnyek = false, bool showScroll = true)
        {
            int eredetix = x;
            int eredetiy = y;
            int kivalasztott = 1;
            bool igaze = true;
            if (hosszusag == 0) hosszusag = elemek.Length;
            if (szelesseg == 0) szelesseg = elemek.OrderBy(asd => asd.Length).First().Length;
            if (merre == Orientation.vertical) Ablak(x, y, szelesseg, hosszusag, BackGround, arnyek, 0);
            else Ablak(x, y, szelesseg * elemek.Length, 1, BackGround, arnyek, 0);
            Console.ForegroundColor = ForeGround;
            while (igaze)
            {
                Console.SetCursorPosition(x, y);
                for (int i = 0; i < hosszusag; i++)
                {
                    if (merre == Orientation.vertical) Console.SetCursorPosition(x, y + i);
                    else if (merre == Orientation.horizontal) Console.SetCursorPosition(x + (i * hosszusag), y);
                    if (i < elemek.Length)
                    {
                        if (kivalasztott > hosszusag)
                        {
                            if (i == hosszusag - 1) Console.BackgroundColor = SelectedMenu;
                            Console.Write(elemek[i + kivalasztott - hosszusag]);
                            for (int k = 0; k < szelesseg - (elemek[i + kivalasztott - hosszusag]).Length; k++) Console.Write(" ");
                        }
                        else
                        {
                            if (kivalasztott - 1 == i) Console.BackgroundColor = SelectedMenu;
                            Console.Write(elemek[i]);
                            for (int k = 0; k < szelesseg - elemek[i].Length; k++) Console.Write(" ");
                        }
                    }
                    Console.BackgroundColor = Menu;
                }
                if (showScroll)//scroll
                {
                    Line('║', hosszusag, x + szelesseg - 1, y, Orientation.vertical, ForeGround, BackGround);
                    Console.SetCursorPosition(x + szelesseg - 1, y);
                    Console.Write('O');
                    Console.SetCursorPosition(x + szelesseg - 1, y + hosszusag - 1);
                    Console.Write('O');
                    try
                    {
                        Console.SetCursorPosition(x + szelesseg - 1, y + 1 + (int)((double)(kivalasztott - 1) / (elemek.Length - 1) * (hosszusag - 3)));
                    }
                    catch (Exception)
                    {
                    }
                    Console.Write('▓');
                }
                Console.SetCursorPosition(x + szelesseg - 1, y);//hogy a listán kívül ne irjon semmit
                //billentyuvárás
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.DownArrow:
                        if ((kivalasztott + 1 <= elemek.Length) && merre == Orientation.vertical)
                        {
                            kivalasztott++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if ((kivalasztott - 1 > 0) && merre == Orientation.vertical)
                        {
                            kivalasztott--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if ((kivalasztott + 1 <= elemek.Length) && merre == Orientation.horizontal)
                        {
                            kivalasztott++;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if ((kivalasztott - 1 > 0) && merre == Orientation.horizontal)
                        {
                            kivalasztott--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        igaze = false;
                        break;
                    case ConsoleKey.Escape:
                        kivalasztott = -1;
                        igaze = false;
                        break;
                    case ConsoleKey.Tab:
                        kivalasztott = -2;
                        igaze = false;
                        break;
                }
                x = eredetix;
                y = eredetiy;
            }
            return kivalasztott;
        }
        //✅️
        public string TextBox(int x, int y, int hossz, ConsoleColor ForeGround, ConsoleColor BackGround, bool passProtected)
        {
            List<char> szoveg = new List<char>();
            string returner = "";
            ConsoleKeyInfo keyInfo;
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ForeGround;
            Console.BackgroundColor = BackGround;
            for (int i = 0; i < hossz; i++) Console.Write(" ");
            Console.SetCursorPosition(x, y);
            do
            {
                Console.SetCursorPosition(x, y);
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (szoveg.Count > 0)
                    {
                        szoveg.Remove(szoveg[szoveg.Count - 1]);
                        Console.SetCursorPosition(x - 1, y);
                        Console.Write(" ");
                        x--;
                    }
                }
                else
                {
                    if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        if (szoveg.Count < hossz)
                        {
                            x++;
                            if (passProtected)
                            {
                                Console.Write("*");
                            }
                            else Console.Write(keyInfo.KeyChar);
                            szoveg.Add(keyInfo.KeyChar);
                        }
                    }
                    else
                    {
                        foreach (char a in szoveg) returner += a;
                        return returner;
                    }
                }
            } while (true);
        }
        //✅️
        public char[,] MTextBox(int x, int y, int szelesseg, int hosszusag, ConsoleColor ForeGround, ConsoleColor BackGround, ConsoleKey kilepes, bool textOverWrite, bool scroll)
        {
            List<char> szoveg = new List<char>();
            char[,] returner = new char[hosszusag, szelesseg];
            ConsoleKeyInfo keyInfo;
            Console.SetCursorPosition(x, y);
            Ablak(x, y, szelesseg, hosszusag, BackGround, false, 0);
            Console.ForegroundColor = ForeGround;
            Console.BackgroundColor = BackGround;
            int holx = 0;
            int holy = 0;
            //for (int i = 0; i < hossz; i++) Console.Write(" ");
            Console.SetCursorPosition(x + holx, y + holy);
            do
            {
                Console.SetCursorPosition(x + holx, y + holy);
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (!(holx == 0 && holy == 0))
                    {
                        if (holx > 0)
                        {
                            holx--;
                            Console.SetCursorPosition(x + holx, y + holy);
                            Console.Write(' ');
                            returner[holy, holx] = '\0';
                            Console.SetCursorPosition(x + holx, y + holy);
                        }
                        else
                        {
                            holx = szelesseg - 1;
                            holy--;
                            Console.SetCursorPosition(x + holx, y + holy);
                            Console.Write(' ');
                            returner[holy, holx] = '\0';
                        }
                        //holx = szelesseg - 1;
                    }

                }
                else
                {
                    //enter kezelése
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        if (holy < hosszusag - 1)
                        {
                            holx = 0;
                            holy++;
                            Console.SetCursorPosition(x + holx, y + holy);
                        }
                        continue;//ez azért kell mer a readKey enterje bezavar
                    }
                    //nyilakkal tudj mozogni mer mier ne
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (holy > 0)
                            {
                                //holx = 0;
                                holy--;
                                Console.SetCursorPosition(x + holx, y + holy);
                                //ez azért kell mer a readKey enterje bezavar
                            }
                            continue;
                        case ConsoleKey.DownArrow:
                            if (holy < hosszusag - 1)
                            {
                                //holx = 0;
                                holy++;
                                Console.SetCursorPosition(x + holx, y + holy);
                                //ez azért kell mer a readKey enterje bezavar
                            }
                            continue;

                        case ConsoleKey.RightArrow:
                            if (holx < szelesseg - 1)
                            {
                                //holx = 0;
                                holx++;
                                Console.SetCursorPosition(x + holx, y + holy);
                                //ez azért kell mer a readKey enterje bezavar
                            }
                            continue;
                        case ConsoleKey.LeftArrow:
                            if (holx > 0)
                            {
                                //holx = 0;
                                holx--;
                                Console.SetCursorPosition(x + holx, y + holy);
                                //ez azért kell mer a readKey enterje bezavar
                            }
                            continue;
                        default:
                            break;
                    }



                    if (keyInfo.Key != kilepes)
                    {
                        if (holx <= szelesseg - 1)
                        {
                            //holx = 0;
                            Console.SetCursorPosition(x + holx, y + holy);
                            Console.Write(keyInfo.KeyChar);
                            returner[holy, holx] = keyInfo.KeyChar;
                            holx++;
                        }
                        else
                        {
                            if (holy <= hosszusag - 2)
                            {
                                holx = 0;
                                holy++;
                                Console.SetCursorPosition(x + holx, y + holy);
                                Console.Write(keyInfo.KeyChar);
                                holx++;
                                returner[holy, holx] = keyInfo.KeyChar;
                            }
                        }
                    }
                    else
                    {
                        return returner;
                    }
                }
            } while (true);
        }
        //✅️
        public void progressBar(int x, int y, int szelesseg, int hossz, ConsoleColor ForeGround, ConsoleColor BackGround, int miliSec)
        {
            Console.ForegroundColor = ForeGround;
            Console.BackgroundColor = BackGround;
            Console.SetCursorPosition(x, y);
            Ablak(x, y, szelesseg, hossz, BackGround, false, 0);
            for (int i = 0; i < 100; i++)
            {
                Ablak(x + (szelesseg * i) / 100, y, 1, hossz, ForeGround, false, 0);
                Thread.Sleep(miliSec);
            }
        }


        //ezek ilyen dummy cuccok, alapok de jó ha vannak, megkönnyítenek sokmindent
        //✅️
        public void TextBlock(string szoveg, int x, int y, ConsoleColor ForeGround, ConsoleColor BackGround)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ForeGround;
            Console.BackgroundColor = BackGround;
            Console.Write(szoveg);
        }

        //✅️
        public void MTextBlock(string[] szovegek, int x, int y, ConsoleColor ForeGround, ConsoleColor BackGround)
        {
            Console.ForegroundColor = ForeGround;
            Console.BackgroundColor = BackGround;

            for (int i = 0; i < szovegek.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(szovegek[i]);
            }
        }

        public void Line(char mibolxd, int hossz, int x, int y, Orientation merre, ConsoleColor ForeGround, ConsoleColor BackGround)
        {
            Console.ForegroundColor = ForeGround;
            Console.BackgroundColor = BackGround;
            if (merre == Orientation.horizontal)
            {
                for (int i = 0; i < hossz; i++)
                {
                    Console.SetCursorPosition(x + i, y);
                    Console.Write(mibolxd);
                }
            }
            else if (merre == Orientation.vertical)
            {
                for (int i = 0; i < hossz; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write(mibolxd);
                }
            }
        }







        //-----------------ezek ilyen easter eggekxd -------------------------
        //                                                                  //
        //--------------------------------------------------------------------
        public void FancyKepernyotorles(int x, int y)
        {// ez tulajdonképpen még a régi józsika kalandjaiban használt metódusom, amugy kb semmire nem hasznáom már
            // de azé menő, nem:D?
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, 0 + 1);
            for (int i = 0; i < y; i++)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                for (int l = 0; l < x; l++) Console.Write("█");
                Thread.Sleep(30);
                if (i <= 0 || i < y)
                {
                    Console.SetCursorPosition(0, i);
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (int b = 0; b < x; b++) Console.Write("█");
                }
                Thread.Sleep(30);
                Console.SetCursorPosition(0, 0 + i);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }

        /// <summary>
        /// MenuPontok(tomb, x, y, terkoz)
        /// </summary>
        public int MenuPontok(string[] elemek, int x, int y, int terkoz)
        {
            int eredetix = x;
            int eredetiy = y;
            int kimenet = 1;
            int kivalasztott = 1;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            bool igaze = true;
            while (igaze)
            {
                Console.SetCursorPosition(x, y);
                for (int i = 0; i < elemek.Length; i++)
                {
                    Console.SetCursorPosition(x, y + i);
                    if (kivalasztott - 1 == i) Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(elemek[i]);

                    y += terkoz;

                    Console.BackgroundColor = ConsoleColor.DarkGray;
                }
                //billentyuvárás
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.DownArrow:
                        if (kivalasztott + 1 <= elemek.Length)
                        {
                            kivalasztott++;
                            kimenet++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (kivalasztott - 1 > 0)
                        {
                            kivalasztott--;
                            kimenet--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        igaze = false;
                        break;
                }
                x = eredetix;
                y = eredetiy;
            }


            return kimenet;
        }

        //szia jovonandi

        //tervezek még ide egy :
        //-textbox-ot (állítható hogy password proof legyen e), de egyenlore csak 1 line
        //-rich textbox (ugyan az mint az elozo, csak multiline. Ez szoposabb, majd kesobb, de van egy kezdetleges kodom hozzá)
        //-valami objektum orientalt rendszert, hogy egy ablakon belül van több dolog, egy button, egy textbox, textblock, esetleg griddel felosztani a jövőben

    }
}