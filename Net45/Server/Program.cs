using Mature.Socket;
using Mature.Socket.Compression;
using Mature.Socket.ContentBuilder;
using Mature.Socket.DataFormat;
using Mature.Socket.Server.DotNetty;
using Mature.Socket.Validation;
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ITCPServer server = new TCPServer(new ContentBuilder(new GZip(), new MD5DataValidation()), new JsonDataFormat(), new MD5DataValidation(), new GZip());
            //ITCPServer server = new TCPServer();//SuperSocket
            server.Start(new ServerConfig
            {
                Port = 2020
            });
            server.NewSessionConnected += Server_NewSessionConnected;
            server.SessionClosed += Server_SessionClosed;
            server.NewRequestReceived += Server_NewRequestReceived;
            Console.ReadLine();
        }
        private static void Server_NewRequestReceived(ISessionWrapper arg1, StringPackageInfo arg2)
        {
            IContentBuilder contentBuilder = new ContentBuilder(new GZip(), new MD5DataValidation());
            Console.WriteLine($"接收到消息，Key：{arg2.Key} Body:{arg2.Body} MessageId:{arg2.MessageId}");
            if (arg2.Key?.Length < 20)
            {
                arg2.Key = arg2.Key.PadRight(20, ' ');
            }
            var data = contentBuilder.Builder(arg2.Key, arg2.Body, arg2.MessageId);
            arg1.Send(data, 0, data.Length);
        }

        private static void Server_SessionClosed(object sender, SessionInfo e)
        {
            Console.WriteLine($"连接断开：{e.SessionID}");
        }

        private static void Server_NewSessionConnected(object sender, SessionInfo e)
        {
            Console.WriteLine($"新连接建立：{e.SessionID}");
        }
    }
}
