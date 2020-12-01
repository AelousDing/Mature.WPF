using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket
{
    public interface ISessionWrapper
    {
        void Send(byte[] data, int offset, int length);
    }
}
