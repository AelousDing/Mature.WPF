using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket
{
    public interface IContentBuilder
    {
        byte[] Builder(string key, string body, string messageId);
        byte[] Builder(string key, string body, string messageId, bool isCompress);
    }
}
