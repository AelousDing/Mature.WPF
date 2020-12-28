using System;
using System.Net;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public interface ITCPClient
    {
        bool IsConnected { get; }
        string SessionId { get; }
        EndPoint RemoteEndPoint { get; }
        Task<bool> ConnectAsync(string ip, ushort port);
        Task<TResponse> SendAsync<TRequest, TResponse>(string key, TRequest request, int timeout);
        void RegisterNotify<TResponse>(string key, Action<TResponse> action);
        void UnRegisterNotify<TResponse>(string key, Action<TResponse> action);
        void Close();
        event EventHandler Connected;
        event EventHandler Closed;
    }
}
