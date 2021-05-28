using Arrowgene.Baf.Server.Core;

namespace Arrowgene.Baf.Server.Packet
{
    public abstract class PacketHandler : IPacketHandler
    {
        protected BafServer _server;
        
        protected PacketHandler(BafServer server)
        {
            _server = server;
        }

        public abstract PacketId Id { get; }
        public abstract void Handle(BafClient client, BafPacket packet);
    }
}