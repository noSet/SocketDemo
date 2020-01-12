using CSocket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace CSocket
{
    public class CScoketServer : IDisposable
    {
        private readonly Socket _mainSocket;

        public IUnpacker Unpack { get; internal set; }

        public Dictionary<Socket, DefaultChannel> Session { get; set; }

        public ITargetBlock<byte[]> TargetBlock { get; set; }

        public CScoketServer(EndPoint endPoint)
        {


            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _mainSocket.Bind(endPoint);
        }

        public void Start()
        {
            _mainSocket.Listen(0);

            _mainSocket.BeginAccept(AcceptAsyncCallback, _mainSocket);
        }

        private void AcceptAsyncCallback(IAsyncResult ar)
        {
            var mainSocket = ar.AsyncState as Socket;
            var session = mainSocket.EndAccept(ar);
            var channel = new DefaultChannel(this, session);
            Session.Add(session, channel);
        }

        public void Stop()
        {
            _mainSocket.Shutdown(SocketShutdown.Both);
            _mainSocket.Close();
        }

        public void Dispose()
        {
            _mainSocket.Dispose();
        }
    }
}
