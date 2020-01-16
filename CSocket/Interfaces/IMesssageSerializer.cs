namespace CSocket.Interfaces
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object message);

        object Deserialize(byte[] message);
    }

    public interface IMesssageSerializer<TMessage>
        : IMessageSerializer
    {
        byte[] Serialize(TMessage message);

        new TMessage Deserialize(byte[] message);
    }
}
