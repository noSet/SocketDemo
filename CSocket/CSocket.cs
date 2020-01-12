using CSocket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CSocket
{
    public class CSocket<TProtocol>
    {
        public ITargetBlock<byte[]> MessageHandling { get; }

        public CSocket(IUnpacker unpacker, IProtocolCoder<TProtocol> protocolCoder, IMessageHandle<TProtocol> messageHandle)
        {
            if (unpacker is null)
            {
                throw new ArgumentNullException(nameof(unpacker));
            }

            if (protocolCoder is null)
            {
                throw new ArgumentNullException(nameof(protocolCoder));
            }

            if (messageHandle is null)
            {
                throw new ArgumentNullException(nameof(messageHandle));
            }

            ExecutionDataflowBlockOptions options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4 };

            TransformManyBlock<byte[], byte[]> transformManyBlock = new TransformManyBlock<byte[], byte[]>(unpacker.Unpack, options);

            TransformBlock<byte[], TProtocol> transformBlock = new TransformBlock<byte[], TProtocol>(protocolCoder.Decoder, options);

            ActionBlock<TProtocol> actionBlock = new ActionBlock<TProtocol>(messageHandle.Handle, options);

            transformManyBlock.LinkTo(transformBlock);
            transformBlock.LinkTo(actionBlock);

            MessageHandling = transformManyBlock;
        }

        public void Send(TProtocol protocol)
        {
            // 处理发送和接收，服务端可以选择发送给哪个客户端，客户端直接发送到服务端
        }
    }
}
