using System;
using System.Net;

namespace Mature.Socket
{
    public class SessionInfo
    {
        public string SessionID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastActiveTime { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
    }
}
