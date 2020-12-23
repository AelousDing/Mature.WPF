using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class HeatBeatHandler : ChannelHandlerAdapter
    {
        public override bool IsSharable => true;
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
            Console.WriteLine(exception.Message);
            context.Channel.CloseAsync();
        }
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent state)
            {
                if (state.State == IdleState.ReaderIdle)
                {
                    context.Channel.CloseAsync();
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }
    }
}
