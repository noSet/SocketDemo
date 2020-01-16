using CSocket.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CSocket
{
    public class DefaultProtocolConverter<TKey, TProtocol> : IProtocolCoder<TKey, TProtocol>
        where TProtocol : IProtocol<TKey>
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public TProtocol Decoder(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] EnCoder(TProtocol message)
        {
            throw new NotImplementedException();
        }

        //public T ReadMessage(byte[] buffer)
        //{
        //    using (MemoryStream stream = new MemoryStream(buffer))
        //    {
        //        Pack<T> pack = _binaryFormatter.Deserialize(stream) as Pack<T>;

        //        return pack.Message;
        //    }
        //}

        //public byte[] WriteMessage(T message)
        //{
        //    byte[] buffer = null;
        //    Pack<T> pack = new Pack<T>() { Message = message };

        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        _binaryFormatter.Serialize(stream, pack);

        //        buffer = stream.ToArray();
        //    }

        //    Array.Copy(BitConverter.GetBytes(buffer.Length), 0, buffer, 4, 4);

        //    return buffer;
        //}
    }
}
