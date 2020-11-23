using Mature.Socket;
using Mature.Socket.Server.SuperSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSocketServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ITCPServer server = new TCPServer();
            server.Start();
            server.NewSessionConnected += Server_NewSessionConnected;
            server.SessionClosed += Server_SessionClosed;
            Console.ReadLine();
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
