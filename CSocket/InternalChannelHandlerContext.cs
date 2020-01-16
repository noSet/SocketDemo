using CSocket.Interfaces;

namespace CSocket
{
    public class InternalChannelHandlerContext<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        public DefaultChannel<TKey, TProtocol> Channel { get; set; }

        public byte[] UnProcessedBytes { get; set; }

        public byte[] ProcessedBytes { get; set; }

        public TKey Code { get; set; }

        public object Message { get; set; }
    }
}
