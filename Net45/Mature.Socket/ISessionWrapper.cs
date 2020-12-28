using System;
using System.Net;

namespace Mature.Socket
{
    public interface ISessionWrapper
    {
        string SessionId { get; }
        EndPoint RemoteEndPoint { get; }
        EndPoint LocalEndPoint { get; }
        void Send(byte[] data, int offset, int length);
    }
}
