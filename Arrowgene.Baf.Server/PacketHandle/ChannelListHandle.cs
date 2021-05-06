using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class ChannelListHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(ChannelListHandle));

        public override PacketId Id => PacketId.ChannelListReq; 

        public override void Handle(BafClient client, BafPacket packet)
        {
        }

    }
}