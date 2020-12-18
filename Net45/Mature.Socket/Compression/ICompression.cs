using System.Text;

namespace Mature.Socket.Compression
{
    public interface ICompression
    {
        byte[] Compress(string content, Encoding encoding);
        byte[] Compress(byte[] content);
        byte[] Decompress(string content, Encoding encoding);
        byte[] Decompress(byte[] content);
    }
}
