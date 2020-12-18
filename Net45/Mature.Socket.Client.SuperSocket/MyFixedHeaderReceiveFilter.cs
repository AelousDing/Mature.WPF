using Mature.Socket.Compression;
using SuperSocket.ProtoBase;
using System;
using System.Linq;
using System.Text;

namespace Mature.Socket.Client.SuperSocket
{
    /*自定义TCP应用协议：
      20字节表示命令  C
      1字节表示报文是否压缩 Z
      4字节表示报文长度 L
      16字节表示数据校验位 V
      报文头共计23字节
     */
    public class MyFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<global::SuperSocket.ProtoBase.StringPackageInfo>
    {
        const int CmdByteCount = 20;
        const int CompressionByteCount = 1;
        const int LengthByteCount = 4;
        const int MessageIdCount = 32;
        const int ValidationIdCount = 8;
        public MyFixedHeaderReceiveFilter() : base(65)
        {

        }
        public override global::SuperSocket.ProtoBase.StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data;
            data = new byte[CmdByteCount];
            bufferStream.Read(data, 0, CmdByteCount);
            var key = Encoding.ASCII.GetString(data);
            data = new byte[CompressionByteCount];
            bufferStream.Read(data, 0, CompressionByteCount);
            bool isCompression = data[0] == 0 ? false : true;
            data = new byte[LengthByteCount];
            bufferStream.Read(data, 0, LengthByteCount);
            var length = BitConverter.ToInt32(data.Reverse().ToArray(), 0);
            data = new byte[MessageIdCount];
            bufferStream.Read(data, 0, MessageIdCount);
            var messageId = Encoding.ASCII.GetString(data);
            data = new byte[ValidationIdCount];
            bufferStream.Read(data, 0, ValidationIdCount);
            byte[] body = null;
            data = new byte[Size - HeaderSize];
            bufferStream.Read(data, 0, Size - HeaderSize);
            if (isCompression)
            {
                ICompression compression = new GZip();
                body = compression.Decompress(data);
            }
            else
            {
                body = data;
            }
            string bodyString = Encoding.UTF8.GetString(body);
            return new global::SuperSocket.ProtoBase.StringPackageInfo(key, bodyString, new string[] { messageId });
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            var bodyLen = BitConverter.ToInt32(bufferStream.Buffers[0].Skip(CmdByteCount+ CompressionByteCount).Take(LengthByteCount).Reverse().ToArray(), 0);
            Console.WriteLine($"接收到报文长度{bodyLen}");
            return bodyLen;
        }
    }
}
