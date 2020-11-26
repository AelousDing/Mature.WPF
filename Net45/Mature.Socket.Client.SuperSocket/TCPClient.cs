﻿using Mature.Socket;
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
            easyClient.Socket.SendTimeout = 30000;
            easyClient.Socket.ReceiveTimeout = 30000;
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
            var isConnect = await easyClient.ConnectAsync(endPoint);
            //启动断线重连

            return isConnect;
        }

        public async Task<string> SendAsync(ushort key, string body, int timeout)
        {
            TaskCompletionSource<StringPackageInfo> taskCompletionSource = new TaskCompletionSource<StringPackageInfo>();
            string messageId = Guid.NewGuid().ToString().Replace("-", "");
            task.TryAdd(messageId, taskCompletionSource);
            //设置超时
            var cts = new CancellationTokenSource();
            StringPackageInfo result = null;
            try
            {
                //cts.CancelAfter(timeout);
                //cts.Token.Register(() => taskCompletionSource.TrySetException(new TimeoutException("请求超时。")));
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
                //cts.Cancel();
                throw ex;
            }
            finally
            {
                task.TryRemove(messageId, out TaskCompletionSource<StringPackageInfo> tcs);
                cts.Dispose();
            }
            return result?.Body;
        }
        public async Task<TResponse> SendAsync<TRequest, TResponse>(ushort key, TRequest request, int timeout)
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
    }
}
