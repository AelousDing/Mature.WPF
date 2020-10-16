using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.SuperSocket.DataFormat
{
    public class JsonDataFormat : IDataFormat
    {
        public string Format<T>(T source)
        {
            return JsonConvert.SerializeObject(source);
        }
    }
}
