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
      2字节表示命令  C
      1字节表示报文是否压缩 Z
      4字节表示报文长度 L
      32字节表示消息ID V
      16字节表示数据校验位 V
      报文头共计9字节
     */
    public class MyFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<StringRequestInfo>
    {
        public MyFixedHeaderReceiveFilter() : base(47)
        {

        }
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            return BitConverter.ToInt32(header.Skip(offset + 3).Take(4).ToArray(), 0);
        }

        protected override StringRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            bool isCompress = header.Array[2] == 0 ? false : true;
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
            var body = Encoding.UTF8.GetString(data);
            var messageId = Encoding.ASCII.GetString(header.Array, 7, 32);
            return new StringRequestInfo(BitConverter.ToUInt16(header.Array, header.Offset).ToString(), body, new string[] { messageId });
        }
    }
}
