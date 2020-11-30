using Mature.Socket.Common.SuperSocket.Compression;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.SuperSocket
{
    /*自定义TCP应用协议：
      20字节表示命令  C
      1字节表示报文是否压缩 Z
      4字节表示报文长度 L
      32字节表示消息ID V
      16字节表示数据校验位 V
      报文头共计9字节
     */
    public class MyFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<StringRequestInfo>
    {
        const int CmdByteCount = 20;
        const int CompressionByteCount = 1;
        const int LengthByteCount = 4;
        const int MessageIdCount = 32;
        const int ValidationIdCount = 8;
        public MyFixedHeaderReceiveFilter() : base(65)
        {

        }
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return BitConverter.ToInt32(header.Skip(offset + CmdByteCount + CompressionByteCount).Take(LengthByteCount).ToArray(), 0);
        }

        protected override StringRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            bool isCompress = header.Array[CmdByteCount] == 0 ? false : true;
            string body = "";
            if (bodyBuffer != null)
            {
                byte[] data = null;
                if (isCompress)
                {
                    //解压缩处理
                    ICompression compression = new GZip();
                    data = compression.Decompress(bodyBuffer.Skip(offset).Take(length).ToArray());
                }
                else
                {
                    data = bodyBuffer.Skip(offset).Take(length).ToArray();
                }
                body = Encoding.UTF8.GetString(data);
            }
            var messageId = Encoding.ASCII.GetString(header.Array, CmdByteCount + CompressionByteCount + LengthByteCount, MessageIdCount);
            return new StringRequestInfo(Encoding.ASCII.GetString(header.Array, header.Offset, CmdByteCount).ToString(), body, new string[] { messageId });
        }
    }
}
