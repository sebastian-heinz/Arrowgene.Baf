using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Packet
{
    public class BafPacket
    {
        private IBuffer _data;
        public ushort Id { get; }

        public BafPacket(IBuffer data)
        {
            _data = data;
        }
    }
}