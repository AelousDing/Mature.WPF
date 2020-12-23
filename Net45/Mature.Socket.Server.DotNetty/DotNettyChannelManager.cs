using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class DotNettyChannelManager
    {
        private DotNettyChannelManager()
        {

        }
        public static readonly object objLock = new object();

        private static DotNettyChannelManager instance;

        public static DotNettyChannelManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new DotNettyChannelManager();
                        }
                    }
                }
                return instance;
            }
        }


        public event EventHandler<SessionInfo> NewSessionConnected;
        public event EventHandler<SessionInfo> SessionClosed;
        public ConcurrentDictionary<string, IChannel> Channels { get; set; } = new ConcurrentDictionary<string, IChannel>();

        public void Add(string key, IChannel value)
        {
            if (!Channels.ContainsKey(key))
            {
                Channels.TryAdd(key, value);
                NewSessionConnected.Invoke(this, new SessionInfo
                {
                    SessionID = value.Id.AsLongText(),
                    LastActiveTime = DateTime.Now,
                    LocalEndPoint = (IPEndPoint)value.LocalAddress,
                    RemoteEndPoint = (IPEndPoint)value.RemoteAddress,
                    StartTime = DateTime.Now
                });
            }
        }
        public void Remove(string key)
        {
            if (Channels.ContainsKey(key))
            {
                Channels.TryRemove(key, out IChannel channel);
                SessionClosed?.Invoke(this, new SessionInfo
                {
                    SessionID = channel.Id.AsLongText(),
                    LastActiveTime = DateTime.Now,
                    LocalEndPoint = (IPEndPoint)channel.LocalAddress,
                    RemoteEndPoint = (IPEndPoint)channel.RemoteAddress,
                });
            }
        }

    }
}
