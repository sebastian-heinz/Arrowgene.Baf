namespace Arrowgene.Baf.Server.Packet
{
    public class BafPacket
    {
        public byte[] Data;
        public ushort Id { get; }

        public BafPacket(ushort id, byte[] data)
        {
            Id = id;
            Data = data;
        }
    }
}