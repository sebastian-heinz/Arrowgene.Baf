using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class JoinChannelHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(ChannelListHandle));

        public override PacketId Id => PacketId.JoinChannelReq;

        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();

            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);


            BafPacket p = new BafPacket(PacketId.JoinChannelRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}