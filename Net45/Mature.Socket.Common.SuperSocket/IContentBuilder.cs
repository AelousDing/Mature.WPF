using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket
{
    public interface IContentBuilder
    {
        byte[] Builder(ushort key, string body);
        byte[] Builder(ushort key, string body, bool isCompress);
    }
}
