using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket
{
    public class ContentBuilder : IContentBuilder
    {
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
