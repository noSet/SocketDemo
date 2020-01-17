using System;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using CSocket.Interfaces;

namespace CSocket
{
    public class DefaultChannel<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        private readonly SocketPipe<TKey, TProtocol> _socketPipe;

        public Socket Socket { get; }

        public byte[] UnProcessed { get; internal set; } = new byte[0];

        public DefaultChannel(Socket socket, SocketPipe<TKey, TProtocol> socketPipe)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _socketPipe = socketPipe ?? throw new ArgumentNullException(nameof(socketPipe));
            byte[] buffers = new byte[1024];
            socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveAsyncCallback, buffers);
        }

        private void ReceiveAsyncCallback(IAsyncResult ar)
        {
            var buffers = ar.AsyncState as byte[];

            if (!Socket.Connected)
            {
                return;
            }

            var length = Socket.EndReceive(ar);

            if (length > 0)
            {
                var data = new byte[length];
                Array.Copy(buffers, data, length);

                _socketPipe.ReceivePipe.Post(new InternalChannelHandlerContext<TKey, TProtocol>() { UnProcessedBytes = data, Channel = this });

                Socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveAsyncCallback, buffers);
            }
        }

        public void SendMessage(TKey key, object message)
        {
            _socketPipe.SendPipe.Post(new InternalChannelHandlerContext<TKey, TProtocol>() { Code = key, Channel = this, Message = message });
        }
    }
}
