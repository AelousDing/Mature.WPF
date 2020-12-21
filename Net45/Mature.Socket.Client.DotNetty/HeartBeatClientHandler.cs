using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Client.DotNetty
{
    public class HeartBeatClientHandler : ChannelHandlerAdapter
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            Console.WriteLine("ChannelActive");
        }
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            Console.WriteLine("ChannelInactive");
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            base.ChannelRead(context, message);
            Console.WriteLine("ChannelRead");
        }
    }
}
