using Mature.Socket;
using Mature.Socket.Common.SuperSocket;
using Mature.Socket.Common.SuperSocket.DataFormat;
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
        IContentBuilder contentBuilder;
        IDataFormat dataFormat;
        public TCPClient(IContentBuilder contentBuilder, IDataFormat dataFormat)
        {
            this.contentBuilder = contentBuilder;
            this.dataFormat = dataFormat;
            easyClient = new EasyClient<StringPackageInfo>();
            easyClient.Initialize(new MyFixedHeaderReceiveFilter());
            easyClient.NewPackageReceived += EasyClient_NewPackageReceived;
            easyClient.Connected += EasyClient_Connected;
            easyClient.Closed += EasyClient_Closed;
        }

        private void EasyClient_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, null);
            }
        }

        private void EasyClient_Connected(object sender, EventArgs e)
        {
            if (Connected != null)
            {
                Connected(this, null);
            }
        }

        private void EasyClient_NewPackageReceived(object sender, PackageEventArgs<StringPackageInfo> e)
        {
            Console.WriteLine($"Key:{e.Package.Key}  Body:{e.Package.Body}");
        }

        EasyClient<StringPackageInfo> easyClient;
        public bool IsConnected => easyClient.IsConnected;

        public event EventHandler Connected;
        public event EventHandler Closed;

        public async Task<bool> ConnectAsync(string ip, ushort port)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return await easyClient.ConnectAsync(endPoint);
        }

        public Task<string> SendAsync(ushort key, string body)
        {
            easyClient.Send(contentBuilder.Builder(key, body));
            throw new NotImplementedException();
        }
        public Task<TResponse> SendAsync<TRequest, TResponse>(ushort key, TRequest request)
        {
            easyClient.Send(contentBuilder.Builder(key, dataFormat.Format<TRequest>(request)));
            throw new NotImplementedException();
        }
    }
}
