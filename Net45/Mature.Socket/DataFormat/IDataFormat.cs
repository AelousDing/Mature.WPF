namespace Mature.Socket.DataFormat
{
    public interface IDataFormat
    {
        string Serialize<T>(T source);
        T Deserialize<T>(string source);
    }
}
