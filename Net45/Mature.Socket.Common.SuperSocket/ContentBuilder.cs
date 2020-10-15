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
    //报文格式：key 压缩标志位 校验位 正文
    public class ContentBuilder : IContentBuilder
    {
        int keyLength = 2;
        int compressFlagLength = 1;
        int bodyLength = 4;
        int validationLength;

        ICompression compression;
        IDataValidation dataValidation;
        public ContentBuilder(ICompression compression, IDataValidation dataValidation)
        {
            this.compression = compression;
            this.dataValidation = dataValidation;
            validationLength = dataValidation.Length;
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
