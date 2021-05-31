using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class RoomChangeSettingHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(RoomChangeSettingHandle));

        public override PacketId Id => PacketId.RoomChangeSettingReq;

        public RoomChangeSettingHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();
            BafPacket p = new BafPacket(PacketId.RoomChangeSettingRes, b.GetAllBytes());
          //  client.Send(p);
        }

    }
}