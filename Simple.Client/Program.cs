using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Simple.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入要发送的内容！");

            string input = Console.ReadLine();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(new IPEndPoint(IPAddress.Loopback, 9933));
            for (int i = 0; i < 100; i++)
            {
                var arr = Encoding.Default.GetBytes(input);

                var size = BitConverter.GetBytes(arr.Length + 4);

                socket.Send(size.Concat(arr).ToArray());
                Console.WriteLine(i);

                Thread.Sleep(100);
            }

            Console.ReadLine();
        }
    }
}
