namespace Mature.Socket.ContentBuilder
{
    public interface IContentBuilder
    {
        byte[] Builder(string key, string body, string messageId);
        byte[] Builder(string key, string body, string messageId, bool isCompress);
        byte[] Builder<TBody>(string key, TBody body, string messageId, bool isCompress);
    }
}
