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
