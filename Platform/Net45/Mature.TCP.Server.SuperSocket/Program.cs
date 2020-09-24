using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.TCP.Server.SuperSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            ITCPServer server = new TCPServer();
            if (!server.Start())
            {
                Console.WriteLine("启动失败,请检查日志确认失败原因");
            }
            Console.ReadLine();
        }
    }
}
