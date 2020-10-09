using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public class SessionInfo
    {
        public string SessionID { get; set; }
        public DateTime StartTime { get; }
        public DateTime LastActiveTime { get; set; }
        public IPEndPoint RemoteEndPoint { get; }
        public IPEndPoint LocalEndPoint { get; }
    }
}
