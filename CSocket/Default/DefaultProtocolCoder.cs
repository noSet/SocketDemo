using System;
using CSocket.Interfaces;

namespace CSocket.Default
{
    public class DefaultProtocolCoder : IProtocolCoder<int, DefaultProtocol>
    {
        public DefaultProtocol Decoder(byte[] data)
        {
            DefaultProtocol protocol = new DefaultProtocol();

            var lengthBytes = new byte[sizeof(int)];
            Array.Copy(data, 0, lengthBytes, 0, 4);

            protocol.Length = BitConverter.ToInt32(lengthBytes, 0);

            var codeBytes = new byte[sizeof(int)];
            Array.Copy(data, 4, codeBytes, 0, 4);

            protocol.Code = BitConverter.ToInt32(codeBytes, 0);

            protocol.Message = new byte[data.Length - 2 * sizeof(int)];
            Array.Copy(data, 8, protocol.Message, 0, protocol.Message.Length);

            return protocol;
        }

        public byte[] EnCoder(DefaultProtocol message)
        {
            byte[] bytes = new byte[message.Message.Length + 2 * sizeof(int)];

            Array.Copy(BitConverter.GetBytes(message.Length), 0, bytes, 0, sizeof(int));

            Array.Copy(BitConverter.GetBytes(message.Code), 0, bytes, 4, sizeof(int));

            Array.Copy(message.Message, 0, bytes, 8, message.Length);

            return bytes;
        }
    }
}
