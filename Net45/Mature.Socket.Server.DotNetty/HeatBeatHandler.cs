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
    public class HeatBeatHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            Console.WriteLine("接收到心跳信号");
            ctx.FireChannelRead(msg);
        }
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
                    Console.WriteLine(" 没有收到客户端的信号 连接断开");
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }
    }
}
