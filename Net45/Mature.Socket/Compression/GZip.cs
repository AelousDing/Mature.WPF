using System.IO;
using System.IO.Compression;
using System.Text;

namespace Mature.Socket.Compression
{
    public class GZip : ICompression
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
                    }
                    //GZipStream存取数据的时候是以4K为块进行存取的。
                    //所以在压缩的时候，在返回stream.ToArray()前应该先gZipStream.Close()（也就是上面俺卖关子的那里），
                    //原因是GZipStream是在Dispose的时候把数据完全写入
                    return stream.ToArray();
                }
            }
        }
        public byte[] Decompress(string content, Encoding encoding)
        {
            byte[] data = encoding.GetBytes(content);
            return Decompress(data);
        }
        public byte[] Decompress(byte[] content)
        {
            using (MemoryStream dataStream = new MemoryStream(content))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(dataStream, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(stream);
                        return stream.ToArray();
                    }
                }
            }
        }
    }
}
