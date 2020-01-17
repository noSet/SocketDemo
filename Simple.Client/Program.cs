using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Simple.Client
{
    class Program
    {
        [Serializable]
        public class Simple
        {
            public int Num { get; set; }

            public Simple2 Simple2 { get; set; }
        }

        [Serializable]
        public class Simple2
        {
            public int Num { get; set; }

            public string Str { get; set; }
        }


        static void Main(string[] args)
        {
            BinaryFormatter _binaryFormatter = new BinaryFormatter();

            Simple simple = new Simple()
            {
                Num = 1,
                Simple2 = new Simple2
                {
                    Num = 2,
                    Str = "aaa"
                }
            };

            MemoryStream stream1 = new MemoryStream();
            _binaryFormatter.Serialize(stream1, simple);
            var data = stream1.ToArray();

            MemoryStream stream2 = new MemoryStream(data);
            var simple2 = _binaryFormatter.Deserialize(stream2) as Simple;

            Console.WriteLine("请输入要发送的内容！");

            string input = Console.ReadLine();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(new IPEndPoint(IPAddress.Loopback, 9933));

            for (int i = 0; i < 100; i++)
            {
                var arr = Serialize(simple);

                var size = BitConverter.GetBytes(arr.Length + 2 * sizeof(int));

                socket.Send(size.Concat(new byte[] { 233, 3, 0, 0 }).Concat(arr).ToArray());
                Console.WriteLine(i);

                Thread.Sleep(100);
            }

            Console.ReadLine();
        }

        public static byte[] Serialize(object message)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BsonDataWriter writer = new BsonDataWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, message);

                return stream.ToArray();
            }
        }
    }
}
