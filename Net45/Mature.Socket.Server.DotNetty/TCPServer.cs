using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Mature.Socket.Common.DotNetty;
using Mature.Socket.Compression;
using Mature.Socket.Config;
using Mature.Socket.ContentBuilder;
using Mature.Socket.DataFormat;
using Mature.Socket.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class TCPServer : ITCPServer
    {
        private const int CmdByteCount = 20;
        private const int CompressionByteCount = 1;
        private const int LengthByteCount = 4;
        private const int MessageIdCount = 32;
        private const int ValidationIdCount = 8;

        public event EventHandler<SessionInfo> NewSessionConnected;
        public event Action<ISessionWrapper, StringPackageInfo> NewRequestReceived;
        public event EventHandler<SessionInfo> SessionClosed;
        IContentBuilder contentBuilder;
        IDataFormat dataFormat;
        IDataValidation dataValidation;
        ICompression compression;
        public TCPServer(IContentBuilder contentBuilder, IDataFormat dataFormat, IDataValidation dataValidation, ICompression compression)
        {
            this.contentBuilder = contentBuilder;
            this.dataFormat = dataFormat;
            this.dataValidation = dataValidation;
            this.compression = compression;
        }
        public IEnumerable<SessionInfo> GetAllSession()
        {
            throw new NotImplementedException();
        }

        public SessionInfo GetSessionByID(string sessionID)
        {
            throw new NotImplementedException();
        }
        IEventLoopGroup bossGroup = new MultithreadEventLoopGroup();
        IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();
        ServerBootstrap bootstrap;
        IChannel boundChannel;
        public bool Start(IServerConfig serverConfig)
        {
            var handler = new FrameHandler(dataValidation, compression);
            handler.Handler += Handler_Handler;
            bootstrap = new ServerBootstrap();
            bootstrap.Group(bossGroup, workerGroup);
            bootstrap.Channel<TcpServerSocketChannel>();
            bootstrap
                .Option(ChannelOption.SoBacklog, 100)
                .Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    //pipeline.AddLast(new ChannelManagerHandler());
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(64 * 1024, CmdByteCount + CompressionByteCount, LengthByteCount, MessageIdCount + ValidationIdCount, 0));
                    pipeline.AddLast(handler);
                    pipeline.AddLast(new LengthFieldBasedFrameEncoder(contentBuilder));
                }));

            boundChannel = bootstrap.BindAsync(serverConfig.Port).Result;
            return boundChannel != null;
        }

        private void Handler_Handler(IChannel channel, StringPackageInfo e)
        {
            NewRequestReceived?.Invoke(new SessionWrapper(channel), e);
        }

        public void Stop()
        {
            try
            {
                boundChannel.CloseAsync().Wait();
            }
            finally
            {
                Task.WaitAll(bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }
    }
}
