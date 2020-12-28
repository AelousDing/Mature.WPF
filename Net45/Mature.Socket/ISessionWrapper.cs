namespace Mature.Socket
{
    public interface ISessionWrapper
    {
        string SessionId { get; }
        void Send(byte[] data, int offset, int length);
    }
}
