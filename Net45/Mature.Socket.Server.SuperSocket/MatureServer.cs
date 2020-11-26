using Mature.Socket.Common.SuperSocket;
using Mature.Socket.Common.SuperSocket.Compression;
using Mature.Socket.Common.SuperSocket.Validation;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.SuperSocket
{
    public class MatureServer : AppServer<MatureSession>
    {
        public MatureServer() : base(new DefaultReceiveFilterFactory<MyFixedHeaderReceiveFilter, StringRequestInfo>())
        {
            NewRequestReceived += MatureServer_NewRequestReceived;
        }

        private void MatureServer_NewRequestReceived(MatureSession session, StringRequestInfo requestInfo)
        {
            System.Threading.Thread.Sleep(50000);
            IContentBuilder contentBuilder = new ContentBuilder(new GZip(), new MD5DataValidation());
            Console.WriteLine($"接收到消息，Key：{requestInfo.Key} Body:{requestInfo.Body} MessageId:{requestInfo.GetFirstParam()}");
            var data = contentBuilder.Builder(ushort.Parse(requestInfo.Key), requestInfo.Body, requestInfo.GetFirstParam());
            session.Send(data, 0, data.Length);
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }
        protected override void OnStarted()
        {
            base.OnStarted();
            Console.WriteLine("OnStarted");
        }
        protected override void OnStopped()
        {
            base.OnStopped();
            Console.WriteLine("OnStopped");
        }
    }
}
