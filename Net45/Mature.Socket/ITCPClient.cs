using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public interface ITCPClient
    {
        bool IsConnected { get; }
        Task<bool> ConnectAsync(string ip, ushort port);
        Task<string> SendAsync(ushort key, string body);
        Task<TResponse> SendAsync<TRequest, TResponse>(ushort key, TRequest request);
        event EventHandler Connected;
        event EventHandler Closed;
    }
}
