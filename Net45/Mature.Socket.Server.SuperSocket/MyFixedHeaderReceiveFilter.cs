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
      2字节表示数据校验位 V
      报文头共计9字节
     */
    public class MyFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<StringRequestInfo>
    {
        public MyFixedHeaderReceiveFilter() : base(9)
        {

        }
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var strLen = Encoding.ASCII.GetString(header, offset + 3, 4);
            return int.Parse(strLen.TrimStart('0'));
        }

        protected override StringRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            bool isCompress = bool.Parse(Encoding.ASCII.GetString(header.Array, 2, 1));
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
            Console.WriteLine(body);
            return new StringRequestInfo(Encoding.ASCII.GetString(header.Array, header.Offset, 2), body, new string[] { body });
        }
    }
}
