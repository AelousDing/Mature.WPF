using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Mature.TCP
{
    public interface ITCPServer
    {
        IEnumerable<SessionInfo> GetAllSession();
        SessionInfo GetSessionByID(string sessionID);
        void Run(ushort port);
    }
}
