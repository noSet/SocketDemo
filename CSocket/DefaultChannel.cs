using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace CSocket
{
    public class DefaultChannel
    {
        private readonly CScoketServer _scoketServer;
        private readonly Socket _socket;

        public DefaultChannel(CScoketServer scoketServer, Socket socket)
        {
            _scoketServer = scoketServer ?? throw new ArgumentNullException(nameof(scoketServer));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));

            byte[] buffers = new byte[8];
            socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveAsyncCallback, buffers);
        }

        private void ReceiveAsyncCallback(IAsyncResult ar)
        {
            var buffers = ar.AsyncState as byte[];

            var length = _socket.EndReceive(ar);

            if (length > 0)
            {
                var data = new byte[length];
                Array.Copy(buffers, data, length);

                _scoketServer.TargetBlock.Post(data);

                _socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveAsyncCallback, buffers);
            }
        }

        public void SendMessage(byte[] buffer)
        {
            _socket.Send(buffer);
        }
    }
}
