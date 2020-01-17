using System;
using System.IO;
using CSocket.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace CSocket.Default
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        public byte[] Serialize(object message)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BsonDataWriter writer = new BsonDataWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, message);

                return stream.ToArray();
            }
        }

        public object Deserialize(byte[] message, Type type)
        {
            using (MemoryStream stream = new MemoryStream(message))
            using (BsonDataReader reader = new BsonDataReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize(reader, type);
            }
        }
    }
}
