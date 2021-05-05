using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Packet
{
    public class BafPacket
    {
        private byte[] _data;
        public ushort Id { get; }

        public BafPacket(ushort id, byte[] data)
        {
            Id = id;
            _data = data;
        }
    }
}