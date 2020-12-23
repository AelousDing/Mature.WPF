using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Mature.Socket.ContentBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Client.DotNetty
{
    public class HeartBeatHandler : ChannelHandlerAdapter
    {
        IContentBuilder contentBuilder;
        public HeartBeatHandler(IContentBuilder contentBuilder)
        {
            this.contentBuilder = contentBuilder;
        }
        public override bool IsSharable => true;
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
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent state)
            {
                if (state.State == IdleState.WriterIdle)
                {
                    context.WriteAndFlushAsync(contentBuilder.Builder("PING", "", Guid.NewGuid().ToString().Replace("-", "")));
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }
    }
}
