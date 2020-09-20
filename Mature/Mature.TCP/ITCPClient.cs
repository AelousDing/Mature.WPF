using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mature.TCP
{
    public interface ITCPClient
    {
        bool IsConnected { get; }
        bool Connect(string ip, ushort port);
        Task<TResponse> Send<TRequest, TResponse>(TRequest request);
        event EventHandler Connected;
        event EventHandler Closed;
    }
}
