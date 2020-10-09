using Mature.Socket;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Client.SuperSocket
{
    public class TCPClient : ITCPClient
    {
        public TCPClient()
        {
            easyClient = new EasyClient<StringPackageInfo>();
            easyClient.Initialize(new MyFixedHeaderReceiveFilter());
            easyClient.NewPackageReceived += EasyClient_NewPackageReceived;
        }

        private void EasyClient_NewPackageReceived(object sender, PackageEventArgs<StringPackageInfo> e)
        {

        }

        EasyClient<StringPackageInfo> easyClient;
        public bool IsConnected => throw new NotImplementedException();

        public event EventHandler Connected;
        public event EventHandler Closed;

        public async Task<bool> ConnectAsync(string ip, ushort port)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return await easyClient.ConnectAsync(endPoint);
        }

        public Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
        {
            easyClient.Send(new byte[] { });
            throw new NotImplementedException();
        }
    }
}
