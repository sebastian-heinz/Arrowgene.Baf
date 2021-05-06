using Arrowgene.Baf.Server.Core;

namespace Arrowgene.Baf.Server.Packet
{
    public abstract class PacketHandler : IPacketHandler
    {
        protected PacketHandler() 
        {
        }

        public abstract PacketId Id { get; }
        public abstract void Handle(BafClient client, BafPacket packet);
    }
}