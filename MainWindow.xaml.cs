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

namespace Template
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int port = 8010;
        private IPAddress ipServer;
        private Socket listeningSocket;
        private EndPoint remotePoint;
        private Task<string> task;
        public  MainWindow()
        {
            InitializeComponent();
            String host = Dns.GetHostName();
            ipServer = Dns.GetHostEntry(host).AddressList[4];
            IPServ.Content = ipServer.ToString();
            Task.Run(() => Listen());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = Mes.Text;
                Socket listening = new Socket(AddressFamily.InterNetwork,
                         SocketType.Dgram, ProtocolType.Udp);
                byte[] data = Encoding.Unicode.GetBytes(message);
                EndPoint remotePoint = new IPEndPoint(IPAddress.Parse(IPClient.Text), port);
                listening.SendTo(data, remotePoint);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Listen()
        {
                  StringBuilder builder = new StringBuilder();
                  IPEndPoint ipPoint = new IPEndPoint(ipServer, port);
                  listeningSocket = new Socket(AddressFamily.InterNetwork,
                      SocketType.Dgram, ProtocolType.Udp);
                  listeningSocket.Bind(ipPoint);
                  try
                  {
                      while (true)
                      {

                          byte[] data = new byte[256];
                          remotePoint = new IPEndPoint(IPAddress.Any, 0);
                          int bytes = 0;
                          do
                          {
                              bytes = listeningSocket.ReceiveFrom(data, ref remotePoint);
                              builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                              builder.Append(remotePoint);
                              builder.Append("\n");
                          }
                          while (listeningSocket.Available > 0);
                          IPEndPoint remoteFullIp = remotePoint as IPEndPoint;
                          Dispatcher.Invoke(()=>Message.Text=builder.ToString());
                      }
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }
                  finally
                  {
                      if (listeningSocket != null)
                      {
                          listeningSocket.Shutdown(SocketShutdown.Both);
                          listeningSocket.Close();
                          listeningSocket = null;
                      }
                  }
        }
    }
}
