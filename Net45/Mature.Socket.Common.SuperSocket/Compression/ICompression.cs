using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket.Compression
{
    public interface ICompression
    {
        byte[] Compress(string content, Encoding encoding);
        byte[] Compress(byte[] content);
        byte[] Decompress(string content, Encoding encoding);
        byte[] Decompress(byte[] content);
    }
}
