using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket.Validation
{
    public class MD5DataValidation : IDataValidation
    {
        public byte[] Validation(byte[] source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var data = md5.ComputeHash(source);
            return data.Skip(4).Take(8).ToArray();
        }
    }
}
