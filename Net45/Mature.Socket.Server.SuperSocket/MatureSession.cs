using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.SuperSocket
{
    public class MatureSession : AppSession<MatureSession>
    {
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
            Console.WriteLine("OnSessionStarted");
        }
        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            Console.WriteLine($"OnSessionClosed CloseReason:{reason}");
        }
        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
            Console.WriteLine("HandleUnknownRequest");
        }
        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
            Console.WriteLine("HandleException");
        }
    }
}
