using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket.DataFormat
{
    public interface IDataFormat
    {
        string Serialize<T>(T source);
        T Deserialize<T>(string source);
    }
}
