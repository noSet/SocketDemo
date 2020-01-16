using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSocket.Interfaces
{
    public interface IMessageHandle<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        void Handle(ChannelHandlerContext<TKey, TProtocol, object> context);
    }

    public interface IMessageHandle<TKey, TProtocol, TMessage> : IMessageHandle<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        void Handle(ChannelHandlerContext<TKey, TProtocol, TMessage> context);
    }
}
