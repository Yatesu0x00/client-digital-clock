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
using System.Windows.Threading;
using System.Threading;

namespace Client1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        myDialog dlg;
        public Socket s { get; set; }
        public IPAddress host { get; set; }
        Thread thread_1;
        private bool connectStatus = false;
        //private string request;
        byte[] bytes;

        public MainWindow()
        {
            InitializeComponent();

            disconnect.IsEnabled = false;           
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            dlg = new myDialog();
            dlg.ShowDialog();            

            if(dlg.DialogResult == true) 
            {
                disconnect.IsEnabled = true;
                connect.IsEnabled = false;
                connectStatus = true;

                host = IPAddress.Parse(dlg.ipAddress);

                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(host, dlg.port);
                //Console.WriteLine("Verbindung erfolgreich");

                thread_1 = new Thread(ThreadFunction);
                thread_1.Start();
            }
        }

        private void ThreadFunction()
        {
            string request = "send_time";

            while (connectStatus == true)
            {
                bytes = new byte[99];             
                
                s.Send(Encoding.ASCII.GetBytes(request));             
               
                s.Receive(bytes);
                string time = Encoding.ASCII.GetString(bytes);

                Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    lbTime.Content = " " + time;
                }));

                Thread.Sleep(50);
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            connectStatus = false;
            //s.Disconnect(true);
            s.Close();
            connect.IsEnabled = true;
            disconnect.IsEnabled = false;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mw_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                thread_1.Abort();
            }
            catch { }
        }
    }
}
