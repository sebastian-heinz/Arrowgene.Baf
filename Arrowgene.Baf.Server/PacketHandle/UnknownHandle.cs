using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class UnknownHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(UnknownHandle));

        public override PacketId Id => PacketId.Unknown;

        public UnknownHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            Logger.Info(client, $"Unhandled PacketId:{packet.IdValue} Hex:{packet.IdValue:X}");
        }

    }
}