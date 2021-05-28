using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class LobbyProfileHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(LobbyProfileHandle));

        public override PacketId Id => PacketId.LobbyProfileReq;

        public LobbyProfileHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();

            b.WriteInt32(0);
            b.WriteCString(client.Character.Name);
            b.WriteByte(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0); // cog Music
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteByte(0);
            b.WriteInt32(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
         
            BafPacket p = new BafPacket(PacketId.LobbyProfileRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}