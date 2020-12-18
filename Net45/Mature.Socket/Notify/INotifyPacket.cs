namespace Mature.Socket.Notify
{
    public interface INotifyPacket
    {
        void Raise(object response);
        bool Equals(INotifyPacket notifyPacket);
    }
}
