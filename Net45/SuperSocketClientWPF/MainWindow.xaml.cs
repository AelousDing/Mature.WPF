using Mature.Socket;
using Mature.Socket.Client.SuperSocket;
using Mature.Socket.Common.SuperSocket;
using Mature.Socket.Common.SuperSocket.Compression;
using Mature.Socket.Common.SuperSocket.DataFormat;
using Mature.Socket.Common.SuperSocket.Validation;
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

namespace SuperSocketClientWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ITCPClient client;
        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            client = new TCPClient(new ContentBuilder(new GZip(), new MD5DataValidation()), new JsonDataFormat());
            bool isConnected = await client.ConnectAsync(tbIp.Text, ushort.Parse(tbPort.Text));
            Console.WriteLine(isConnected ? "连接成功" : "连接失败");
        }
        const ushort TestCmd = 0x01;
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await client.SendAsync(TestCmd, tbSend.Text, 30000);
                tbReceive.Text += result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                tbReceive.Text += ex.Message;
            }
        }
    }
}
