using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Server.DotNetty
{
    public class ChannelManagerHandler : ChannelHandlerAdapter
    {
        public override bool IsSharable => true;
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            DotNettyChannelManager.Instance.Add(context.Channel.Id.AsLongText(), context.Channel);

        }
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            DotNettyChannelManager.Instance.Remove(context.Channel.Id.AsLongText());
        }
    }
}
