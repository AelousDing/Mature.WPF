namespace Mature.Socket
{
    public interface ISessionWrapper
    {
        void Send(byte[] data, int offset, int length);
    }
}
