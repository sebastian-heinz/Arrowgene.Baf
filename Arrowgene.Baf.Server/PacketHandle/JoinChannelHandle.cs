using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class JoinChannelHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(ChannelListHandle));

        public override PacketId Id => PacketId.JoinChannelReq;

        public JoinChannelHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            short channelTab = buffer.ReadInt16();
            short channelNumber = buffer.ReadInt16();

            Channel channel = _server.GetChannel(channelTab, channelNumber);
            client.Channel = channel;
            
            IBuffer b = new StreamBuffer();
            b.WriteInt32(0); // unknown
            BafPacket p = new BafPacket(PacketId.JoinChannelRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}