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
        ConcurrentDictionary<string, List<INotifyPacket>> notify = new ConcurrentDictionary<string, List<INotifyPacket>>();
        public TCPClient(IContentBuilder contentBuilder, IDataFormat dataFormat)
        {
            this.contentBuilder = contentBuilder;
            this.dataFormat = dataFormat;
            easyClient = new EasyClient<StringPackageInfo>();
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

        private void EasyClient_NewPackageReceived(object sender, PackageEventArgs<StringPackageInfo> e)
        {
            Console.WriteLine($"Key:{e.Package.Key}  Body:{e.Package.Body}");

            if (task.TryGetValue(e.Package.GetFirstParam(), out TaskCompletionSource<StringPackageInfo> tcs))
            {
                tcs.TrySetResult(e.Package);
            }
            else
            {
                if (notify.ContainsKey(e.Package.Key))
                {
                    foreach (var item in notify[e.Package.Key])
                    {
                        item.Raise(e.Package);
                    }
                }
            }
        }

        EasyClient<StringPackageInfo> easyClient;

        public System.Net.Sockets.Socket Socket => easyClient?.Socket;

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
            TaskCompletionSource<StringPackageInfo> taskCompletionSource = new TaskCompletionSource<StringPackageInfo>();
            string messageId = Guid.NewGuid().ToString().Replace("-", "");
            task.TryAdd(messageId, taskCompletionSource);
            StringPackageInfo result = null;
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
                task.TryRemove(messageId, out TaskCompletionSource<StringPackageInfo> tcs);
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
            if (notify.ContainsKey(key))
            {
                notify[key].Add(new NotifyPacket<TResponse>(action));
            }
            else
            {
                notify.TryAdd(key, new List<INotifyPacket>
                {
                    new NotifyPacket<TResponse>(action)
                });
            }
        }

        public void UnRegisterNotify<TResponse>(string key, Action<TResponse> action)
        {
            if (notify.ContainsKey(key))
            {
                INotifyPacket notifyPacket = null;
                foreach (var item in notify[key])
                {
                    if (item.Equals(action))
                    {
                        notifyPacket = item;
                        break;
                    }
                }
                if (notifyPacket != null)
                {
                    notify[key].Remove(notifyPacket);
                }
            }
        }
    }
}
