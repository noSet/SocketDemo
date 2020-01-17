using System;
using System.Collections.Generic;
using CSocket.Interfaces;

namespace CSocket
{
    public static class Builder
    {
        public class ProtocolOption<TKey, TProtocol>
            where TProtocol : IProtocol<TKey>
        {
            public IProtocolUnpacker Unpacker { get; set; }

            public IProtocolCoder<TKey, TProtocol> ProtocolCoder { get; set; }
        }

        public static BuilderCore<TKey, TProtocol> UseProtocol<TKey, TProtocol>(Action<ProtocolOption<TKey, TProtocol>> steupOptions)
            where TProtocol : IProtocol<TKey>
        {
            if (steupOptions is null)
            {
                throw new ArgumentNullException(nameof(steupOptions));
            }

            ProtocolOption<TKey, TProtocol> option = new ProtocolOption<TKey, TProtocol>();
            steupOptions(option);

            return new BuilderCore<TKey, TProtocol>(option.Unpacker, option.ProtocolCoder);
        }

        public class BuilderCore<TKey, TProtocol>
            where TProtocol : IProtocol<TKey>
        {
            private readonly IProtocolUnpacker _unpacker;
            private readonly IProtocolCoder<TKey, TProtocol> _protocolCoder;
            private IMessageSerializer _messageSerializer;
            private readonly Dictionary<TKey, Type> _messageMapping = new Dictionary<TKey, Type>();
            private readonly Dictionary<TKey, IMessageHandle<TKey, TProtocol>> _messageHandle = new Dictionary<TKey, IMessageHandle<TKey, TProtocol>>();

            internal BuilderCore(IProtocolUnpacker unpacker, IProtocolCoder<TKey, TProtocol> coder)
            {
                _unpacker = unpacker;
                _protocolCoder = coder;
            }

            public BuilderCore<TKey, TProtocol> UseMessageSerializer(IMessageSerializer messageSerializer)
            {
                if (messageSerializer is null)
                {
                    throw new ArgumentNullException(nameof(messageSerializer));
                }

                _messageSerializer = messageSerializer;

                return this;
            }

            public BuilderCore<TKey, TProtocol> RegistMessageHandle<TMessage>(TKey key, IMessageHandle<TKey, TProtocol, TMessage> messageHandle)
            {
                if (messageHandle is null)
                {
                    throw new ArgumentNullException(nameof(messageHandle));
                }

                _messageMapping.Add(key, typeof(TMessage));
                _messageHandle.Add(key, messageHandle);

                return this;
            }

            public CScoketServer<TKey, TProtocol> Build()
            {
                SocketPipe<TKey, TProtocol> cSocket = new SocketPipe<TKey, TProtocol>(_unpacker, _protocolCoder, _messageMapping, _messageSerializer, _messageHandle);

                return new CScoketServer<TKey, TProtocol>(cSocket);
            }
        }
    }
}
