using Mature.Socket.Common.SuperSocket;
using Mature.Socket.Common.SuperSocket.DataFormat;
using Mature.Socket.Notify;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mature.Socket.Client.SuperSocket
{
    public class TCPClient : ITCPClient
    {
        IContentBuilder contentBuilder;
        IDataFormat dataFormat;
        ConcurrentDictionary<string, TaskCompletionSource<global::SuperSocket.ProtoBase.StringPackageInfo>> task = new System.Collections.Concurrent.ConcurrentDictionary<string, TaskCompletionSource<global::SuperSocket.ProtoBase.StringPackageInfo>>();

        public TCPClient(IContentBuilder contentBuilder, IDataFormat dataFormat)
        {
            this.contentBuilder = contentBuilder;
            this.dataFormat = dataFormat;
            easyClient = new EasyClient<global::SuperSocket.ProtoBase.StringPackageInfo>();
            easyClient.KeepAliveTime = 60;//单位：秒
            easyClient.KeepAliveInterval = 5;//单位：秒
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

        private void EasyClient_NewPackageReceived(object sender, PackageEventArgs<global::SuperSocket.ProtoBase.StringPackageInfo> e)
        {
            Console.WriteLine($"Key:{e.Package.Key}  Body:{e.Package.Body}");

            if (task.TryGetValue(e.Package.GetFirstParam(), out TaskCompletionSource<global::SuperSocket.ProtoBase.StringPackageInfo> tcs))
            {
                tcs.TrySetResult(e.Package);
            }
            else
            {
                NotifyContainer.Instance.Raise(e.Package.Key);
            }
        }

        EasyClient<global::SuperSocket.ProtoBase.StringPackageInfo> easyClient;

        public event EventHandler Connected;
        public event EventHandler Closed;

        public async Task<bool> ConnectAsync(string ip, ushort port)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var isConnect = await easyClient.ConnectAsync(endPoint);
            return isConnect;
        }

        public async Task<string> SendAsync(string key, string body, int timeout)
        {
            if (string.IsNullOrEmpty(key) || key.Length >= 20)
            {
                throw new Exception("The key length is no more than 20.");
            }
            else
            {
                key = key.PadRight(20, ' ');
            }
            TaskCompletionSource<global::SuperSocket.ProtoBase.StringPackageInfo> taskCompletionSource = new TaskCompletionSource<global::SuperSocket.ProtoBase.StringPackageInfo>();
            //超时处理
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Token.Register(() => taskCompletionSource.TrySetException(new TimeoutException()));
            cts.CancelAfter(timeout);
            string messageId = Guid.NewGuid().ToString().Replace("-", "");
            task.TryAdd(messageId, taskCompletionSource);
            global::SuperSocket.ProtoBase.StringPackageInfo result = null;
            try
            {
                Console.WriteLine($"发送消息，消息ID：{messageId} 消息命令标识：{key} 消息内容：{body}");
                easyClient.Send(contentBuilder.Builder(key, body, messageId));
                result = await taskCompletionSource.Task;
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
                throw ex;
            }
            finally
            {
                cts.Dispose();
                task.TryRemove(messageId, out TaskCompletionSource<global::SuperSocket.ProtoBase.StringPackageInfo> tcs);
            }
            return result?.Body;
        }
        public async Task<TResponse> SendAsync<TRequest, TResponse>(string key, TRequest request, int timeout)
        {
            string body = await SendAsync(key, dataFormat.Serialize<TRequest>(request), timeout);
            return dataFormat.Deserialize<TResponse>(body);
        }

        public void Close()
        {
            if (Closed != null)
            {
                Closed(this, null);
            }
        }

        public void RegisterNotify<TResponse>(string key, Action<TResponse> action)
        {
            NotifyContainer.Instance.Register<TResponse>(key, action);
        }

        public void UnRegisterNotify<TResponse>(string key, Action<TResponse> action)
        {
            NotifyContainer.Instance.UnRegister<TResponse>(key, action);
        }
    }
}
