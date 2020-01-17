using CSocket;
using CSocket.Default;
using CSocket.Interfaces;
using System;
using System.Net;

namespace Simple.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            CScoketServer<int, DefaultProtocol> server = Builder.UseProtocol<int, DefaultProtocol>(options =>
               {
                   options.Unpacker = new DefaultUnpacker();
                   options.ProtocolCoder = new DefaultProtocolCoder();
               })
            .UseMessageSerializer(new DefaultMessageSerializer())
            .RegistMessageHandle(1001, new PrintHandle())
            .Build();

            server.Start(new IPEndPoint(IPAddress.Any, 9933));

            Console.ReadLine();
        }

        public class PrintHandle : IMessageHandle<int, DefaultProtocol, Simple>
        {
            public void Handle(ChannelHandlerContext<int, DefaultProtocol, Simple> context)
            {
                Console.WriteLine(context.Message);
            }

            public void Handle(ChannelHandlerContext<int, DefaultProtocol, object> context)
            {
                Console.WriteLine(context.Message);
            }
        }

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
    }
}
