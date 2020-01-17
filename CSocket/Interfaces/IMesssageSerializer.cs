using System;

namespace CSocket.Interfaces
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object message);

        object Deserialize(byte[] message, Type type);
    }
}
