using System;
using CSocket.Interfaces;

namespace CSocket
{
    public class ChannelHandlerContext<TKey, TProtocol, TMessage>
        where TProtocol : IProtocol<TKey>
    {
        public DefaultChannel<TKey, TProtocol> Channel { get; }

        public TMessage Message { get; }

        public ChannelHandlerContext(DefaultChannel<TKey, TProtocol> channel, TMessage message)
        {
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Message = message;
        }
    }
}
