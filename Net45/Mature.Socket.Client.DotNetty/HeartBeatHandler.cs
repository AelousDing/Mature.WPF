using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Client.DotNetty
{
    public class HeartBeatHandler : SimpleChannelInboundHandler<IByteBuffer>
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
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent state)
            {
                if (state.State == IdleState.WriterIdle)
                {
                    context.WriteAndFlushAsync(context.Allocator.DirectBuffer().WriteString("", Encoding.UTF8));
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            ctx.FireChannelRead(msg);
        }
    }
}
