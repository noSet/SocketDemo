namespace CSocket.Protocol
{
    public class DefaultProtocol
    {
        public int Version { get; set; }

        public int Code { get; set; }

        public byte[] Message { get; set; }
    }
}
