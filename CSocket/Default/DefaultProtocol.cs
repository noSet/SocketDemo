using CSocket.Interfaces;

namespace CSocket.Default
{
    public class DefaultProtocol : IProtocol<int>
    {
        public int Lenth { get; set; }

        public int Code { get; set; }

        public byte[] Message { get; set; }
    }
}
