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
        public MyFixedHeaderReceiveFilter() : base(9)
        {

        }
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            throw new NotImplementedException();
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            throw new NotImplementedException();
        }
    }
}
