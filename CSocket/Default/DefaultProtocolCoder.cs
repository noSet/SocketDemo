using CSocket.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CSocket.Default
{
    public class DefaultProtocolCoder : IProtocolCoder<int, DefaultProtocol>
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public DefaultProtocol Decoder(byte[] data)
        {
            //using (MemoryStream stream = new MemoryStream(data))
            //{
            //    DefaultProtocol pack = _binaryFormatter.Deserialize(stream) as Pack<T>;

            //    return pack.Message;
            //}

            throw new NotImplementedException();
        }

        public byte[] EnCoder(DefaultProtocol message)
        {
            //byte[] buffer = null;
            //Pack<T> pack = new Pack<T>() { Message = message };

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    _binaryFormatter.Serialize(stream, pack);

            //    buffer = stream.ToArray();
            //}

            //Array.Copy(BitConverter.GetBytes(buffer.Length), 0, buffer, 4, 4);

            //return buffer;
            throw new NotImplementedException();
        }
    }
}
