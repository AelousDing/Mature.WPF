using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Mature.TCP
{
    public interface ITCPServer
    {
        ConcurrentDictionary<string, ITCPClient> Connections { get; set; }
        bool Listening(ushort port);
    }
}
