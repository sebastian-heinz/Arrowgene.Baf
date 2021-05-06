using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Packet
{
    public class BafPacket
    {
        public byte[] Data;
        public PacketId Id { get; }

        public BafPacket(PacketId id, byte[] data)
        {
            Id = id;
            Data = data;
        }

        public IBuffer CreateBuffer()
        {
            return new StreamBuffer(Data);
        }
    }
}