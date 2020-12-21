using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class SessionWrapper : ISessionWrapper
    {
        IChannel channel;
        public SessionWrapper(IChannel channel)
        {
            this.channel = channel;
        }
        public void Send(byte[] data, int offset, int length)
        {
            channel.WriteAndFlushAsync(data);
        }
    }
}
