using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Simple.Server
{
    class Program
    {
        public static Dictionary<Socket, ReceiveHandle> Sockets { get; } = new Dictionary<Socket, ReceiveHandle>();

        static void Main(string[] args)
        {


            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 9933));
            socket.Listen(0);

            while (true)
            {
                try
                {
                    Socket session = socket.Accept();
                    Sockets.Add(session, new ReceiveHandle(session));
                    byte[] buffers = new byte[8];
                    session.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveCallBack, new SimpleData(session, buffers));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("连接错误");
                    Console.WriteLine(ex);
                }
            }
        }

        public static void ReceiveCallBack(IAsyncResult ar)
        {
            SimpleData simpleData = ar.AsyncState as SimpleData;

            var socket = simpleData.Socket;
            var buffers = simpleData.Buffers;

            var length = socket.EndReceive(ar);

            if (length > 0)
            {
                ReceiveHandle handle = Sockets[socket];

                var data = new byte[length];
                Array.Copy(buffers, data, length);

                handle.Handle(data);

                socket.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, ReceiveCallBack, new SimpleData(socket, buffers));
            }
        }

        public class ReceiveHandle
        {
            private byte[] _message = new byte[0];

            public Socket Socket { get; }

            public ReceiveHandle(Socket socket)
            {
                Socket = socket;
            }

            public void Handle(byte[] data)
            {
                var oldLength = _message.Length;
                Array.Resize(ref _message, _message.Length + data.Length);
                Array.Copy(data, 0, _message, oldLength, data.Length);

                处理新信息();
            }

            public void 处理新信息()
            {
                // 小于4不处理
                if (_message.Length < 4)
                {
                    return;
                }

                var size = new byte[4];

                Array.Copy(_message, 0, size, 0, 4);

                // 包没读完 不处理
                if (BitConverter.ToInt32(size, 0) > _message.Length)
                {
                    return;
                }

                var message = new byte[BitConverter.ToInt32(size, 0)];

                Array.Copy(_message, 0, message, 0, message.Length);

                Console.WriteLine(new Message(message));

                var newMessage = new byte[_message.Length - message.Length];

                Array.Copy(_message, message.Length , newMessage, 0, _message.Length - message.Length);

                _message = newMessage;

                处理新信息();
            }
        }

        public class SimpleData
        {
            public Socket Socket { get; }

            public byte[] Buffers { get; }

            public SimpleData(Socket socket, byte[] buffers)
            {
                Socket = socket;
                Buffers = buffers;
            }
        }

        public class Message
        {
            private static int _count;
            public string MSG { get; }

            public Message(byte[] message)
            {
                MSG = Encoding.Default.GetString(message, 4, message.Length - 4);
            }

            public static implicit operator string(Message message) => message.MSG + ++_count;
        }
    }
}
