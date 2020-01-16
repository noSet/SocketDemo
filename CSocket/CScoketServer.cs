using CSocket.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CSocket
{
    public class CScoketServer<TKey, TProtocol> : IDisposable
        where TProtocol : IProtocol<TKey>
    {
        private readonly Socket _mainSocket;
        private SocketPipe<TKey, TProtocol> _cSocket;

        public List<DefaultChannel<TKey, TProtocol>> Channels { get; set; }

        public CScoketServer(SocketPipe<TKey, TProtocol> cSocket)
        {
            _cSocket = cSocket ?? throw new ArgumentNullException(nameof(cSocket));
            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(EndPoint endPoint)
        {
            _mainSocket.Bind(endPoint);

            _mainSocket.Listen(0);

            _mainSocket.BeginAccept(AcceptAsyncCallback, _mainSocket);

            void AcceptAsyncCallback(IAsyncResult ar)
            {
                var mainSocket = ar.AsyncState as Socket;
                var session = mainSocket.EndAccept(ar);
                var channel = new DefaultChannel<TKey, TProtocol>(session, _cSocket);
                Channels.Add(channel);
            }
        }

        public void Stop()
        {
            // todo 这个Socket关闭后，是否需要将客户端连接的Socket也关闭
            _mainSocket.Shutdown(SocketShutdown.Both);
            _mainSocket.Close();

            foreach (var channel in Channels)
            {
                channel.Socket.Shutdown(SocketShutdown.Both);
                _mainSocket.Close();
            }
        }

        public void Send(TKey code, object obj, params DefaultChannel<TKey, TProtocol>[] channels)
        {
            foreach (var channel in channels)
            {
                channel.SendMessage(code, obj);
            }
        }

        public void Dispose()
        {
            _mainSocket.Dispose();

            foreach (var channel in Channels)
            {
                channel.Socket.Dispose();
            }
        }
    }
}
