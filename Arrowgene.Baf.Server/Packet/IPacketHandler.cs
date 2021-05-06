using Arrowgene.Baf.Server.Core;

namespace Arrowgene.Baf.Server.Packet
{
    public interface IPacketHandler
    {
        PacketId Id { get; }
        void Handle(BafClient client, BafPacket packet);
    }
}