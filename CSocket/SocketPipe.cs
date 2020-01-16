using CSocket.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace CSocket
{
    public class SocketPipe<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        private readonly IProtocolUnpacker _unpacker;
        private readonly IProtocolCoder<TKey, TProtocol> _protocolCoder;
        private readonly IMesssageSerializer<object> _defaultSerializer;
        private readonly Dictionary<TKey, IMessageSerializer> _messageSerializer = new Dictionary<TKey, IMessageSerializer>();
        private readonly Dictionary<TKey, IMessageHandle<TKey, TProtocol>> _messageHandle = new Dictionary<TKey, IMessageHandle<TKey, TProtocol>>();

        public ITargetBlock<byte[]> MessageHandling { get; }

        public ITargetBlock<InternalChannelHandlerContext<TKey, TProtocol>> SendPipe { get; }

        public ITargetBlock<InternalChannelHandlerContext<TKey, TProtocol>> ReceivePipe { get; }

        public SocketPipe(
            IProtocolUnpacker unpacker,
            IProtocolCoder<TKey, TProtocol> protocolCoder,
            IMesssageSerializer<object> defaultSerializer,
            Dictionary<TKey, IMessageSerializer> messageSerializer,
            Dictionary<TKey, IMessageHandle<TKey, TProtocol>> messageHandle)
        {
            _unpacker = unpacker ?? throw new ArgumentNullException(nameof(unpacker));
            _protocolCoder = protocolCoder ?? throw new ArgumentNullException(nameof(protocolCoder));
            _defaultSerializer = defaultSerializer ?? throw new ArgumentNullException(nameof(defaultSerializer));
            _messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
            _messageHandle = messageHandle ?? throw new ArgumentNullException(nameof(messageHandle));

            SendPipe = InitSend();
            ReceivePipe = InitReceive();
        }

        public ITargetBlock<InternalChannelHandlerContext<TKey, TProtocol>> InitSend()
        {
            return null;
        }

        public ITargetBlock<InternalChannelHandlerContext<TKey, TProtocol>> InitReceive()
        {
            ExecutionDataflowBlockOptions options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4 };

            var upPakc = new TransformManyBlock<InternalChannelHandlerContext<TKey, TProtocol>, InternalChannelHandlerContext<TKey, TProtocol>>(UnPack, options);
            var decoder = new TransformBlock<InternalChannelHandlerContext<TKey, TProtocol>, InternalChannelHandlerContext<TKey, TProtocol>>(Decoder, options);
            var deserialize = new TransformBlock<InternalChannelHandlerContext<TKey, TProtocol>, InternalChannelHandlerContext<TKey, TProtocol>>(Deserialize, options);
            var messageHandle = new ActionBlock<InternalChannelHandlerContext<TKey, TProtocol>>(MessageHandle, options);
            var failHandle = new ActionBlock<InternalChannelHandlerContext<TKey, TProtocol>>(FailHandle, options);

            upPakc.LinkTo(decoder, context => context != null);
            upPakc.LinkTo(failHandle);

            decoder.LinkTo(deserialize, context => context != null);
            decoder.LinkTo(failHandle);

            deserialize.LinkTo(messageHandle, context => context != null);
            deserialize.LinkTo(failHandle);

            return upPakc;
        }

        /// <summary>
        /// 拆包
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IEnumerable<InternalChannelHandlerContext<TKey, TProtocol>> UnPack(InternalChannelHandlerContext<TKey, TProtocol> context)
        {
            try
            {
                var message = context.Channel.UnProcessed.Concat(context.UnProcessedBytes).ToArray();
                IEnumerable<byte[]> processed = _unpacker.Unpack(message, out var unProcessed);
                context.Channel.UnProcessed = unProcessed;

                return processed.Select(m => new InternalChannelHandlerContext<TKey, TProtocol> { Channel = context.Channel, UnProcessedBytes = m });
            }
            catch (Exception)
            {
                return new InternalChannelHandlerContext<TKey, TProtocol>[] { null };
            }
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private InternalChannelHandlerContext<TKey, TProtocol> Decoder(InternalChannelHandlerContext<TKey, TProtocol> context)
        {
            try
            {
                TProtocol protocol = _protocolCoder.Decoder(context.UnProcessedBytes);
                context.Code = protocol.Code;
                context.ProcessedBytes = protocol.Message;

                return context;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private InternalChannelHandlerContext<TKey, TProtocol> Deserialize(InternalChannelHandlerContext<TKey, TProtocol> context)
        {
            try
            {
                if (_messageSerializer.TryGetValue(context.Code, out var serializer))
                {
                    context.Message = serializer.Deserialize(context.ProcessedBytes);
                }
                else
                {
                    context.Message = _defaultSerializer.Deserialize(context.ProcessedBytes);
                }

                return context;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void MessageHandle(InternalChannelHandlerContext<TKey, TProtocol> context)
        {
            try
            {
                if (!_messageHandle.ContainsKey(context.Code))
                {
                    return;
                }

                var messageHandle = _messageHandle[context.Code];

                var types = messageHandle.GetType().GetGenericArguments();

                var channelHandlerContextType = typeof(ChannelHandlerContext<,,>).MakeGenericType(typeof(TKey), typeof(TProtocol), types[1]);

                var ctor = channelHandlerContextType.GetConstructor(new[] { context.Channel.GetType(), context.Message.GetType() });

                var channelHandlerContext = ctor.Invoke(new[] { context.Channel, context.Message });

                var method = messageHandle.GetType().GetMethod("Handle");

                method.Invoke(messageHandle, new[] { channelHandlerContext });
            }
            catch (Exception ex)
            {
                // todo log
                Debug.WriteLine($"消息处理失败！{context.Channel.Socket.LocalEndPoint} {context.Channel.Socket.RemoteEndPoint} 退出连接！{ex}");
            }
        }

        private void FailHandle(InternalChannelHandlerContext<TKey, TProtocol> context)
        {
            Debug.WriteLine($"消息接收失败！{context.Channel.Socket.LocalEndPoint} {context.Channel.Socket.RemoteEndPoint} 退出连接！");

            context.Channel.Socket.Shutdown(SocketShutdown.Both);
            context.Channel.Socket.Close();
        }
    }
}
