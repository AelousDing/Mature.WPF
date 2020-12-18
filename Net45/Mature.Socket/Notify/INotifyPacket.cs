using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Notify
{
    public interface INotifyPacket
    {
        void Raise(object response);
        bool Equals(INotifyPacket notifyPacket);
    }
}
