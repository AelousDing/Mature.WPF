using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Mature.Socket.Compression;
using Mature.Socket.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Common.DotNetty
{
    public class FrameHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private const int CmdByteCount = 20;
        private const int CompressionByteCount = 1;
        private const int LengthByteCount = 4;
        private const int MessageIdCount = 32;
        private const int ValidationIdCount = 8;
        IDataValidation dataValidation;
        public event Action<IChannel, StringPackageInfo> Handler;
        ICompression compression;
        public FrameHandler(IDataValidation dataValidation, ICompression compression)
        {
            this.dataValidation = dataValidation;
            this.compression = compression;
        }
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            string key = msg.ReadString(CmdByteCount, Encoding.ASCII);
            bool isCompressed = msg.ReadBoolean();
            int length = msg.ReadInt();
            string messageId = msg.ReadString(MessageIdCount, Encoding.ASCII);
            IByteBuffer validation = msg.ReadBytes(ValidationIdCount);
            IByteBuffer bodyBuf = msg.ReadBytes(length);
            byte[] md5 = dataValidation.Validation(bodyBuf.GetIoBuffer().ToArray());
            byte[] validationBuf = validation.GetIoBuffer().ToArray();
            if (!md5.SequenceEqual(validationBuf))
            {
                return;//校验不通过，丢弃数据
            }
            byte[] bodySource = bodyBuf.GetIoBuffer().ToArray();
            if (isCompressed)//解压缩
            {
                bodySource = compression.Decompress(bodySource);
            }
            string body = Encoding.UTF8.GetString(bodySource);
            ReferenceCountUtil.Release(msg);
            Handler?.Invoke(ctx.Channel, new StringPackageInfo
            {
                Key = key?.Trim(),
                Body = body,
                MessageId = messageId
            });
        }
    }
}
