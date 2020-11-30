using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public interface ITCPClient
    {
        System.Net.Sockets.Socket Socket { get; }
        Task<bool> ConnectAsync(string ip, ushort port);
        Task<string> SendAsync(string key, string body, int timeout);
        Task<TResponse> SendAsync<TRequest, TResponse>(string key, TRequest request, int timeout);
        void Close();
        event EventHandler Connected;
        event EventHandler Closed;
    }
}
