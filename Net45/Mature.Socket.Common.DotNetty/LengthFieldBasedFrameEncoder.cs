using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Mature.Socket.ContentBuilder;
using System.Text;

namespace Mature.Socket.Common.DotNetty
{
    public class LengthFieldBasedFrameEncoder : MessageToByteEncoder<StringPackageInfo>
    {
        IContentBuilder contentBuilder;
        public LengthFieldBasedFrameEncoder(IContentBuilder contentBuilder)
        {
            this.contentBuilder = contentBuilder;
        }
        protected override void Encode(IChannelHandlerContext context, StringPackageInfo message, IByteBuffer output)
        {
            if (message.Key.Length < 20)
            {
                message.Key.PadRight(20, ' ');
            }
            IByteBufferAllocator allocator = context.Allocator;
            IByteBuffer byteBuffer = allocator.DirectBuffer();
            contentBuilder.Builder(message.Key, message.Body, message.MessageId, message.IsCompressed);
            byteBuffer.WriteBytes(Encoding.ASCII.GetBytes(message.Key));
            byteBuffer.WriteBoolean(message.IsCompressed);
            byte[] body = Encoding.UTF8.GetBytes(message.Body);
            if (message.IsCompressed)
            {

            }


        }
    }
}
