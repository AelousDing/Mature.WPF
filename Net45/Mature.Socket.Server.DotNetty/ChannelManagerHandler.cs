using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class ChannelManagerHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            
        }
    }
}
