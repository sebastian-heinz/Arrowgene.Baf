using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class RoomLeaveHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(RoomLeaveHandle));

        public override PacketId Id => PacketId.RoomLeaveReq;

        public RoomLeaveHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();
            BafPacket p = new BafPacket(PacketId.RoomLeaveRes, b.GetAllBytes());
          //  client.Send(p);
        }

    }
}