using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket
{
    public class GZipUtil
    {
        public byte[] Compress(string content, Encoding encoding)
        {
            byte[] data = encoding.GetBytes(content);
            return Compress(data);
        }
        public byte[] Compress(byte[] content)
        {
            using (MemoryStream dataStream = new MemoryStream(content))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        dataStream.CopyTo(gZipStream);
                        return stream.ToArray();
                    }
                }
            }
        }
        public byte[] Decompress(string content)
        {

        }
        public byte[] Decompress(byte[] content)
        {

        }
    }
}
