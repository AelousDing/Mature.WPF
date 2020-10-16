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
    //报文格式：key（2位）压缩标志位（1位）报文长度（4位）校验位（16位）正文
    public class ContentBuilder : IContentBuilder
    {
        ICompression compression;
        IDataValidation dataValidation;
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public ContentBuilder(ICompression compression, IDataValidation dataValidation)
        {
            this.compression = compression;
            this.dataValidation = dataValidation;
        }
        public byte[] Builder(ushort key, string body)
        {
            return Builder(key, body, false);
        }

        public byte[] Builder(ushort key, string body, bool isCompress)
        {
            byte[] bodyBuffer = Encoding.GetBytes(body);
            var validation = dataValidation.Validation(bodyBuffer);
            if (isCompress)
            {
                //GZipStream
                bodyBuffer = compression.Compress(bodyBuffer);
            }
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(key));
            data.AddRange(BitConverter.GetBytes(isCompress));
            data.AddRange(BitConverter.GetBytes(bodyBuffer.Length));
            data.AddRange(validation);
            data.AddRange(bodyBuffer);
            return data.ToArray();
        }
    }
}
