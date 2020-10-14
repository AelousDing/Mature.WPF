using Mature.Socket.Common.SuperSocket.Compression;
using Mature.Socket.Common.SuperSocket.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket
{
    //数据完整性校验
    //数据压缩
    public class ContentBuilder : IContentBuilder
    {
        public ContentBuilder(ICompression compression, IDataValidation<string> dataValidation)
        {

        }
        public byte[] Builder(string key, string body)
        {
            return Builder(key, body, false);
        }

        public byte[] Builder(string key, string body, bool isCompress)
        {
            byte[] bodyBuffer = Encoding.UTF8.GetBytes(body);
            if (isCompress)
            {
                //GZipStream
            }
            else
            {

            }
            return bodyBuffer;
        }
    }
}
