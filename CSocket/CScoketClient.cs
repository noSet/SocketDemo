using System;
using System.Net;
using System.Net.Sockets;

namespace CSocket
{
    public class CScoketClient
    {
        private readonly Socket _mainSocket;
        private readonly EndPoint _endPoint;

        public CScoketClient(EndPoint endPoint)
        {
            if (endPoint is null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            _endPoint = endPoint;
            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            _mainSocket.Connect(_endPoint);
        }


    }
}
