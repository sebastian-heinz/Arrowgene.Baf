using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class RoomStartSongHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(RoomStartSongHandle));

        public override PacketId Id => PacketId.RoomStartSongReq;

        public RoomStartSongHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();
            b.WriteInt32(0);
            b.WriteBytes(new byte[54]);
            
            BafPacket p = new BafPacket(PacketId.RoomStartSongRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}