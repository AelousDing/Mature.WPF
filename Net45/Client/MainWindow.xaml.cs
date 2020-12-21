using Mature.Socket;
using Mature.Socket.Client.DotNetty;
using Mature.Socket.Compression;
using Mature.Socket.ContentBuilder;
using Mature.Socket.DataFormat;
using Mature.Socket.Validation;
using System;
using System.Windows;

namespace Client
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
            try
            {
                client = new TCPClient(new ContentBuilder(new GZip(), new MD5DataValidation()), new JsonDataFormat(), new MD5DataValidation(), new GZip());
                bool isConnected = await client.ConnectAsync(tbIp.Text, ushort.Parse(tbPort.Text));
                Console.WriteLine(isConnected ? "连接成功" : "连接失败");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        const string TestCmd = "Request";
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
