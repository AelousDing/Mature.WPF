using System;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public interface ITCPClient
    {
        Task<bool> ConnectAsync(string ip, ushort port);
        Task<string> SendAsync(string key, string body, int timeout);
        Task<TResponse> SendAsync<TRequest, TResponse>(string key, TRequest request, int timeout);
        void RegisterNotify<TResponse>(string key, Action<TResponse> action);
        void UnRegisterNotify<TResponse>(string key, Action<TResponse> action);
        void Close();
        event EventHandler Connected;
        event EventHandler Closed;
    }
}
