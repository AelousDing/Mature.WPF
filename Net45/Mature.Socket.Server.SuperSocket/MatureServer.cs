using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.TCP.Server
{
    public class MatureServer : AppServer<MatureSession>
    {
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
