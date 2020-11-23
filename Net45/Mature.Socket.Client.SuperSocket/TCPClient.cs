using Mature.Socket;
using Mature.Socket.Common.SuperSocket;
using Mature.Socket.Common.SuperSocket.DataFormat;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mature.Socket.Client.SuperSocket
{
    public class TCPClient : ITCPClient
    {
        IContentBuilder contentBuilder;
        IDataFormat dataFormat;
        ConcurrentDictionary<string, TaskCompletionSource<StringPackageInfo>> task = new System.Collections.Concurrent.ConcurrentDictionary<string, TaskCompletionSource<StringPackageInfo>>();
        public int Timeout { get; set; } = 30000;
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
            Console.WriteLine("EasyClient_Closed");
            if (Closed != null)
            {
                Closed(this, null);
            }
        }

        private void EasyClient_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("EasyClient_Connected");
            if (Connected != null)
            {
                Connected(this, null);
            }
        }

        private void EasyClient_NewPackageReceived(object sender, PackageEventArgs<StringPackageInfo> e)
        {
            Console.WriteLine($"Key:{e.Package.Key}  Body:{e.Package.Body}");

            if (task.TryGetValue(e.Package.GetFirstParam(), out TaskCompletionSource<StringPackageInfo> tcs))
            {
                tcs.TrySetResult(e.Package);
            }
        }

        EasyClient<StringPackageInfo> easyClient;
        public bool IsConnected => easyClient.IsConnected;

        public event EventHandler Connected;
        public event EventHandler Closed;

        public async Task<bool> ConnectAsync(string ip, ushort port)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var rrr = await easyClient.ConnectAsync(endPoint);
            byte[] m_KeepAliveOptionValues;
            uint dummy = 0;
            m_KeepAliveOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            //whether enable KeepAlive
            BitConverter.GetBytes((uint)1).CopyTo(m_KeepAliveOptionValues, 0);
            //how long will start first keep alive
            BitConverter.GetBytes((uint)(60 * 1000)).CopyTo(m_KeepAliveOptionValues, Marshal.SizeOf(dummy));
            //keep alive interval
            BitConverter.GetBytes((uint)(60 * 1000)).CopyTo(m_KeepAliveOptionValues, Marshal.SizeOf(dummy) * 2);



            easyClient.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, m_KeepAliveOptionValues);
            return rrr;
        }

        public async Task<string> SendAsync(ushort key, string body)
        {
            TaskCompletionSource<StringPackageInfo> taskCompletionSource = new TaskCompletionSource<StringPackageInfo>();
            string messageId = Guid.NewGuid().ToString().Replace("-", "");
            task.TryAdd(messageId, taskCompletionSource);
            //设置超时
            var cts = new CancellationTokenSource();
            StringPackageInfo result = null;
            try
            {
                cts.CancelAfter(Timeout);
                cts.Token.Register(() => taskCompletionSource.TrySetException(new TimeoutException("请求超时。")));
                Console.WriteLine($"发送消息，消息ID：{messageId} 消息命令标识：{key} 消息内容：{body}");
                easyClient.Send(contentBuilder.Builder(key, body, messageId));
                result = await taskCompletionSource.Task;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("请求超时。");
            }
            catch (Exception ex)
            {
                cts.Cancel();
                throw ex;
            }
            finally
            {
                task.TryRemove(messageId, out TaskCompletionSource<StringPackageInfo> tcs);
                cts.Dispose();
            }
            return result?.Body;
        }
        public async Task<TResponse> SendAsync<TRequest, TResponse>(ushort key, TRequest request)
        {
            string body = await SendAsync(key, dataFormat.Serialize<TRequest>(request));
            return dataFormat.Deserialize<TResponse>(body);
        }
    }
}
