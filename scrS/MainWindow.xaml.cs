using Microsoft.Win32;
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
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace scrS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static int count = 0;
        //public static int loadedImagesCount = 0;
        public MainWindow()
        {

            InitializeComponent();


            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            Directory.SetCurrentDirectory("..");
            Directory.SetCurrentDirectory("dataFromClients");


        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(@".\", "*.png");
                if (files.Length > count)
                {
                    await Task.Delay(500); 

                    try
                    {
                        screenImage.Source = new BitmapImage(
                            new Uri(System.IO.Path.Combine(
                                Directory.GetCurrentDirectory(),
                                files.OrderBy(f => f).Last()
                            ))
                        );
                        //loadedImagesCount++;
                        //text.Text = $"loadedImagesCount : {loadedImagesCount}";
                        //count = files.Length; 
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
