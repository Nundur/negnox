using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading;
using System.Windows.Markup;
using System.Diagnostics.Contracts;
namespace MouseC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static int PORT = 9876;
        public static UdpClient udp = new UdpClient();


        public static List<TcpClient> kliensek = new List<TcpClient>();
        public static bool LeftOnClick = false;
        public static bool RightOnClick = false;
        public static bool LeftOffClick = false;
        public static bool RightOffClick = false;

        public static int elozoX = 0;
        public static int elozoY = 0;

        public static bool kuldeUzenet = false;
        public static string uzenet = "";
        public MainWindow()
        {

            InitializeComponent();
            
            //_ = listen();

            //Rect asd = System.Windows.SystemParameters.WorkArea;

            var vS = SystemInformation.VirtualScreen;

            this.Width = vS.Width;
            this.Height = vS.Height;
            this.Top = vS.Top;
            this.Left = vS.Left;


            ideKattints.MouseLeftButtonDown += IdeKattints_MouseLeftButtonDown;
            ideKattints.MouseLeftButtonUp += IdeKattints_MouseLeftButtonUp;
            ideKattints.MouseRightButtonDown += IdeKattints_MouseRightButtonDown;
            ideKattints.MouseRightButtonUp += IdeKattints_MouseRightButtonUp;

            this.PreviewTextInput += MainWindow_PreviewTextInput;

            this.MouseDown += MainWindow_MouseDown;
            this.KeyDown += MainWindow_KeyDown;
            this.Closing += MainWindow_Closing;


            //this.Loaded += (s, e) => this.Focus();

            _ = listen();
            _ = Sender(this);
        }

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            uzenet = e.Text;
            kuldeUzenet = true;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Send("off|");
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) uzenet = "{ENTER}";
            if (e.Key == Key.Escape) uzenet = "{ESC}";
            if (e.Key == Key.Return) uzenet = "{BS}";
            if (e.Key == Key.Tab) uzenet = "{TAB}";

            if (e.Key == Key.Up) { uzenet = "{UP}"; };
            if (e.Key == Key.Down) uzenet = "{DOWN}";
            if (e.Key == Key.Left) uzenet = "{LEFT}";
            if (e.Key == Key.Right) uzenet = "{RIGHT}";
            kuldeUzenet = true;
        }

        private void IdeKattints_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            RightOffClick = true;
        }

        private void IdeKattints_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightOnClick = true;
        }

        private void IdeKattints_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LeftOffClick = true;
        }

        private void IdeKattints_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.MessageBox.Show("asd");
            LeftOnClick = true;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Send("melegvagy");
        }

        
        static async Task listen()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 3232);
            listener.Start();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[Listener elindult...]");
            Console.ForegroundColor = ConsoleColor.Gray;

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                kliensek.Add(client);
                //kulon szalon monitorozom hogy lecsatlakozott e
                //igen ez egy gyengeseg, ha valaki rájön, engem ki tud fagyasztani a picsaba
                _ = Task.Run(() => MonitorClient(client));
            }
        }
        private static async Task MonitorClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[1];

                // Nem várunk valódi adatot, csak azt figyeljük, él-e még
                while (await stream.ReadAsync(buffer, 0, buffer.Length) > 0)
                {
                }
            }
            catch
            {
            }
            finally
            {
                kliensek.Remove(client);
                client.Close();
            }
        }
        static async Task Send(string message)
        {

            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data = Encoding.UTF8.GetBytes(message);
            await udp.SendAsync(data, data.Length, broadCastEP);
            /*
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in kliensek.ToList())
            {
                try
                {
                    client.GetStream().Write(data, 0, data.Length);
                    //Log($"[÷{message} ÷|elküldve ide :| ÷{((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()}÷|-nek|]");
                }
                catch
                {
                    kliensek.Remove(client);
                }
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            */




        }





        static async Task Sender(MainWindow asd)
        {

            bool Ron = false;
            bool Roff = false;
            bool Lon = false;
            bool Loff = false;


            bool R = false;
            bool L = false;
            var pos = System.Windows.Forms.Cursor.Position;
            string returner = $"{pos.X}|{pos.Y}|{Ron}|{Roff}|{Lon}|{Loff}|";
            while (true)
            {
                await Task.Delay(20);
                pos = System.Windows.Forms.Cursor.Position;


                if (kuldeUzenet)
                {
                    //Send($"keystroke {uzenet}");
                    kuldeUzenet = false;
                }

                //x|y|Ron|Roff|Lon|Roff 


                if (RightOnClick)
                {
                    //Send($"ROnC");
                    RightOnClick = false;
                    R = true;
                }

                if (RightOffClick)
                {
                    //Send($"ROffC");
                    RightOffClick = false;
                    R = false;
                }

                if (LeftOnClick)
                {
                    //Send($"LOnC");
                    LeftOnClick = false;
                    L = true;
                }
                if (LeftOffClick)
                {
                    //Send($"LOffC");
                    LeftOffClick = false;
                    L = false;
                }


                if (elozoX != pos.X || elozoY != pos.Y)
                {
                    //Send($"{pos.X}-{pos.Y}");
                }

                string current = $"{pos.X}|{pos.Y}|{R}|{L}|";

                if (current != returner)
                {
                    await Send(current);
                    returner = current;
                }
                //returner = $"{pos.X}|{pos.Y}|{R}|{L}|";
                //elozoX = pos.X;
                //elozoY = pos.Y;




            }


            /*
            while (true)
            {
                await Task.Delay(20);
                var pos = System.Windows.Forms.Cursor.Position;
                //$"{pos.X}-{pos.Y}"
                if (kuldeUzenet)
                {
                    Send($"keystroke {uzenet}|");
                    kuldeUzenet = false;
                }


                if (RightOffClick)
                {
                    Send("ROffC|");
                    RightOffClick = false;
                }
                if (RightOnClick)
                {
                    Send("ROnC|");
                    RightOnClick = false;
                }
                if (LeftOffClick)
                {
                    Send("LOffC|");
                    LeftOffClick = false;
                }
                if (LeftOnClick)
                {
                    Send("ROnC|");
                    LeftOnClick = false;
                }

                if (elozoX != pos.X || elozoY != pos.Y)
                {
                    Send($"{pos.X}-{pos.Y}|");
                }
                elozoX = pos.X;
                elozoY = pos.Y;

            }
            */
        }
    }
}
