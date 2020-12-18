using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Notify
{
    public class NotifyPacket<T> : INotifyPacket
    {
        Action<T> action;
        public NotifyPacket(Action<T> action)
        {
            this.action = action;
        }
        public void Raise(object response)
        {
            if (action != null)
            {
                action((T)response);
            }
        }
        public bool Equals(INotifyPacket notifyPacket)
        {
            if (action == (notifyPacket as NotifyPacket<T>).action)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
