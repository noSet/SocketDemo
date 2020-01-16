using CSocket.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace CSocket
{
    public class DefaultChannel<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        private readonly SocketPipe<TKey, TProtocol> _cSocket;

        public Socket Socket { get; }

        public byte[] UnProcessed { get; internal set; }

        public DefaultChannel(Socket socket, SocketPipe<TKey, TProtocol> _cSocket)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this._cSocket = _cSocket;
            byte[] buffers = new byte[1024];
            socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveAsyncCallback, buffers);
        }

        private void ReceiveAsyncCallback(IAsyncResult ar)
        {
            var buffers = ar.AsyncState as byte[];

            var length = Socket.EndReceive(ar);

            if (length > 0)
            {
                var data = new byte[length];
                Array.Copy(buffers, data, length);

                _cSocket.ReceivePipe.Post(new InternalChannelHandlerContext<TKey, TProtocol>() { UnProcessedBytes = data, Channel = this });

                Socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveAsyncCallback, buffers);
            }
        }

        public void SendMessage(TKey key, object message)
        {
            _cSocket.SendPipe.Post(new InternalChannelHandlerContext<TKey, TProtocol>() { Code = key, Channel = this, Message = message });
        }
    }
}
