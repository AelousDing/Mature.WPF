namespace Mature.Socket.Server.SuperSocket
{
    public class SessionWrapper : ISessionWrapper
    {
        MatureSession session;
        public SessionWrapper(MatureSession session)
        {
            this.session = session;
        }
        public void Send(byte[] data, int offset, int length)
        {
            session.Send(data, offset, length);
        }
    }
}
