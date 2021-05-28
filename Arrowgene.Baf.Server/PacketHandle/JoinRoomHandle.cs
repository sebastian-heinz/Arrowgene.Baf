using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class JoinRoomHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(JoinRoomHandle));

        public override PacketId Id => PacketId.JoinRoomReq;

        public JoinRoomHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            int number = buffer.ReadInt32();
            int unk = buffer.ReadInt32();
            
            Logger.Debug($"Join Room: Number:{number}");
            Logger.Debug($"unk:{unk}");
            
            IBuffer b = new StreamBuffer();

            b.WriteInt32(number);
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

            BafPacket p = new BafPacket(PacketId.JoinRoomRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}