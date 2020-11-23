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
        public T Deserialize<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public string Serialize<T>(T source)
        {
            return JsonConvert.SerializeObject(source);
        }
    }
}
