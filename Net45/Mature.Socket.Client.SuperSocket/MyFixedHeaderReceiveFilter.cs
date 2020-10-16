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
      2字节表示数据校验位 V
      报文头共计9字节
     */
    public class MyFixedHeaderReceiveFilter : FixedHeaderReceiveFilter<StringPackageInfo>
    {
        public MyFixedHeaderReceiveFilter() : base(23)
        {

        }
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            ushort key = bufferStream.ReadUInt16();
            bool isCompression = bufferStream.ReadByte() == 0 ? false : true;
            byte[] body = null;
            if (isCompression)
            {
                ICompression compression = new GZip();
                body = compression.Decompress(bufferStream.Skip(23).Take(Size - 23)[0].ToArray());
            }
            else
            {
                body = bufferStream.Skip(23).Take(Size - 23)[0].ToArray();
            }
            string bodyString = Encoding.UTF8.GetString(body);
            return new StringPackageInfo(key.ToString(), bodyString, new string[] { bodyString });
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            var bodyLen = bufferStream.Skip(3).ReadInt32();
            return bodyLen;
        }
    }
}
