using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Mature.Socket.Common.DotNetty;
using Mature.Socket.Compression;
using Mature.Socket.ContentBuilder;
using Mature.Socket.DataFormat;
using Mature.Socket.Notify;
using Mature.Socket.Validation;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mature.Socket.Client.DotNetty
{
    public class TCPClient : ITCPClient
    {
        private const int CmdByteCount = 20;
        private const int CompressionByteCount = 1;
        private const int LengthByteCount = 4;
        private const int MessageIdCount = 32;
        private const int ValidationIdCount = 8;

        public event EventHandler Connected;
        public event EventHandler Closed;

        IContentBuilder contentBuilder;
        IDataFormat dataFormat;
        IDataValidation dataValidation;
        ICompression compression;
        ConcurrentDictionary<string, TaskCompletionSource<StringPackageInfo>> task = new ConcurrentDictionary<string, TaskCompletionSource<StringPackageInfo>>();
        public TCPClient(IContentBuilder contentBuilder, IDataFormat dataFormat, IDataValidation dataValidation, ICompression compression)
        {
            this.contentBuilder = contentBuilder;
            this.dataFormat = dataFormat;
            this.dataValidation = dataValidation;
            this.compression = compression;
        }
        public void Close()
        {
            try
            {
                channel?.CloseAsync().Wait();
                Closed?.Invoke(this, null);
            }
            finally
            {
                group?.ShutdownGracefullyAsync().Wait(1000);
            }
        }
        IChannel channel;
        MultithreadEventLoopGroup group;

        public bool IsConnected => channel == null ? false : channel.Active;

        public async Task<bool> ConnectAsync(string ip, ushort port)
        {
            var handler = new FrameHandler(dataValidation, compression);
            handler.Handler += Handler_Handler;
            group = new MultithreadEventLoopGroup();
            var bootstrap = new Bootstrap();
            bootstrap.Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(chaneel =>
                {
                    IChannelPipeline pipeline = chaneel.Pipeline;
                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast(new IdleStateHandler(0, 60, 0));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(64 * 1024, CmdByteCount + CompressionByteCount, LengthByteCount, MessageIdCount + ValidationIdCount, 0));
                    pipeline.AddLast(handler);
                    pipeline.AddLast(new ByteArrayEncoder());
                    pipeline.AddLast(new HeartBeatHandler(contentBuilder));
                }));
            channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
            bool isConnected = (channel != null);
            if (isConnected)
            {
                Connected?.Invoke(this, null);
            }
            return isConnected;
        }
        private void Handler_Handler(IChannel channel, StringPackageInfo e)
        {
            Console.WriteLine($"Key:{e.Key}  Body:{e.Body}");

            if (task.TryGetValue(e.MessageId, out TaskCompletionSource<StringPackageInfo> tcs))
            {
                tcs.TrySetResult(e);
            }
            else
            {
                NotifyContainer.Instance.Raise(e.Key);
            }
        }

        public void RegisterNotify<TResponse>(string key, Action<TResponse> action)
        {
            NotifyContainer.Instance.Register<TResponse>(key, action);
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
            //超时处理
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Token.Register(() => taskCompletionSource.TrySetException(new TimeoutException()));
            cts.CancelAfter(timeout);
            string messageId = Guid.NewGuid().ToString().Replace("-", "");
            task.TryAdd(messageId, taskCompletionSource);
            StringPackageInfo result = null;
            try
            {
                Console.WriteLine($"发送消息，消息ID：{messageId} 消息命令标识：{key} 消息内容：{body}");
                await channel.WriteAndFlushAsync(contentBuilder?.Builder(key, body, messageId, false));
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
                task.TryRemove(messageId, out TaskCompletionSource<StringPackageInfo> tcs);
            }
            return result?.Body;
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(string key, TRequest request, int timeout)
        {
            string body = await SendAsync(key, dataFormat.Serialize<TRequest>(request), timeout);
            return dataFormat.Deserialize<TResponse>(body);
        }

        public void UnRegisterNotify<TResponse>(string key, Action<TResponse> action)
        {
            NotifyContainer.Instance.UnRegister<TResponse>(key, action);
        }
    }
}
