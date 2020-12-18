using Newtonsoft.Json;

namespace Mature.Socket.DataFormat
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
