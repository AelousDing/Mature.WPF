using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class ServerConfig : Mature.Socket.Config.IServerConfig
    {
        public int Port { get; set; }
    }
}
