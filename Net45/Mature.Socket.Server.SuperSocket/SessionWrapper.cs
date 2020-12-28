using System;
using System.Net;

namespace Mature.Socket.Server.SuperSocket
{
    public class SessionWrapper : ISessionWrapper
    {
        MatureSession session;
        public SessionWrapper(MatureSession session)
        {
            this.session = session;
        }

        public string SessionId => session.SessionID;
        public EndPoint RemoteEndPoint { get => session.RemoteEndPoint; }
        public EndPoint LocalEndPoint { get => session.LocalEndPoint; }

        public void Send(byte[] data, int offset, int length)
        {
            session.Send(data, offset, length);
        }
    }
}
