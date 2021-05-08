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

        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();

            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            
            b.WriteByte(3);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            
            b.WriteByte(0);
            b.WriteByte(1);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0x25);
            b.WriteByte(0);
            b.WriteByte(0xD5);
            b.WriteByte(7);
            b.WriteByte(3);

            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0x78);
            b.WriteByte(0x61);
            b.WriteByte(0x2D);
            b.WriteByte(0x6D);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0xFB);
            b.WriteByte(0xD);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);


            BafPacket p = new BafPacket(PacketId.JoinRoomRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}