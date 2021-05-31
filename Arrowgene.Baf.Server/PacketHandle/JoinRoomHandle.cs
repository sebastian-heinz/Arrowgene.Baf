using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class JoinRoomHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(JoinRoomHandle));

        public override PacketId Id => PacketId.RoomJoinReq;

        public JoinRoomHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            int number = buffer.ReadInt32();
            int unk = buffer.ReadInt32();

            Logger.Debug(client, $"Join Room: Number:{number} unk:{unk}");

            IBuffer b = new StreamBuffer();

            b.WriteInt32(0); // TODO
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteInt32(0);
            b.WriteCString("name");
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteInt32(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteCString("name 3");
            b.WriteInt32(0);
            b.WriteInt32(0);

            BafPacket p = new BafPacket(PacketId.RoomJoinRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}