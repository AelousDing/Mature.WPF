using Mature.Socket.Common.SuperSocket.Compression;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Client.SuperSocket
{
    /*自定义TCP应用协议：
      2字节表示命令  C
      1字节表示报文是否压缩 Z
      4字节表示报文长度 L
      16字节表示数据校验位 V
      报文头共计23字节
     */
    public class MyFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<StringPackageInfo>
    {
        const int CmdByteCount = 2;
        const int CompressionByteCount = 1;
        const int LengthByteCount = 4;
        const int MessageIdCount = 32;
        const int ValidationIdCount = 8;
        public MyFixedHeaderReceiveFilter() : base(47)
        {

        }
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data;
            data = new byte[CmdByteCount];
            bufferStream.Read(data, 0, CmdByteCount);
            ushort key = BitConverter.ToUInt16(data, 0);
            data = new byte[CompressionByteCount];
            bufferStream.Read(data, 0, CompressionByteCount);
            bool isCompression = data[0] == 0 ? false : true;
            data = new byte[LengthByteCount];
            bufferStream.Read(data, 0, LengthByteCount);
            var length = BitConverter.ToInt32(data, 0);
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
            return new StringPackageInfo(key.ToString(), bodyString, new string[] { messageId });
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            var bodyLen = BitConverter.ToInt32(bufferStream.Buffers[0].Skip(3).Take(4).ToArray(), 0);
            Console.WriteLine($"接收到报文长度{bodyLen}");
            return bodyLen;
        }
    }
}
