using CSocket.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CSocket
{
    public static class Builder
    {
        public class ProtocolOption<TKey, TProtocol>
            where TProtocol : IProtocol<TKey>
        {
            public IProtocolUnpacker Unpacker { get; set; }

            public IProtocolCoder<TKey, TProtocol> Coder { get; set; }
        }

        public static BuilderCore<TKey, TProtocol> UseProtocol<TProtocol, TKey>(Action<ProtocolOption<TKey, TProtocol>> steupOptions)
            where TProtocol : IProtocol<TKey>
        {
            if (steupOptions is null)
            {
                throw new ArgumentNullException(nameof(steupOptions));
            }

            ProtocolOption<TKey, TProtocol> option = new ProtocolOption<TKey, TProtocol>();
            steupOptions(option);

            return new BuilderCore<TKey, TProtocol>(option.Unpacker, option.Coder);
        }

        public class BuilderCore<TKey, TProtocol>
            where TProtocol : IProtocol<TKey>
        {
            private readonly IProtocolUnpacker _unpacker;
            private readonly IProtocolCoder<TKey, TProtocol> _protocolCoder;
            private readonly Dictionary<TKey, IMessageSerializer> _messageSerializer = new Dictionary<TKey, IMessageSerializer>();
            private readonly Dictionary<TKey, IMessageHandle<TKey, TProtocol>> _messageHandle = new Dictionary<TKey, IMessageHandle<TKey, TProtocol>>();
            private IMesssageSerializer<object> _defaultSerializer;

            internal BuilderCore(IProtocolUnpacker unpacker, IProtocolCoder<TKey, TProtocol> coder)
            {
                _unpacker = unpacker;
                _protocolCoder = coder;
            }

            public BuilderCore<TKey, TProtocol> UseDefaultSerializer(IMesssageSerializer<object> serializer)
            {
                if (serializer is null)
                {
                    throw new ArgumentNullException(nameof(serializer));
                }

                _defaultSerializer = serializer;

                return this;
            }

            public BuilderCore<TKey, TProtocol> RegistMessageSerializer<TMessage>(TKey key, IMesssageSerializer<TMessage> serializer)
            {
                if (serializer is null)
                {
                    throw new ArgumentNullException(nameof(serializer));
                }

                _messageSerializer.Add(key, serializer);

                return this;
            }

            public BuilderCore<TKey, TProtocol> RegistMessageHandle<TMessage>(TKey key, IMessageHandle<TKey, TProtocol, TMessage> messageHandle)
            {
                if (messageHandle is null)
                {
                    throw new ArgumentNullException(nameof(messageHandle));
                }

                _messageHandle.Add(key, messageHandle);

                return this;
            }

            public CScoketServer<TKey, TProtocol> Build()
            {
                SocketPipe<TKey, TProtocol> cSocket = new SocketPipe<TKey, TProtocol>(_unpacker, _protocolCoder, _defaultSerializer, _messageSerializer, _messageHandle);

                return new CScoketServer<TKey, TProtocol>(cSocket);
            }
        }
    }
}
