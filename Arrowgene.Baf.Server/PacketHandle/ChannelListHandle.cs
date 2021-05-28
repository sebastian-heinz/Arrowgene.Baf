using System.Collections.Generic;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class ChannelListHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(ChannelListHandle));

        public override PacketId Id => PacketId.ChannelListReq;

        public ChannelListHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
            List<Channel> channels = _server.GetChannels();

            IBuffer b = new StreamBuffer();
            b.WriteInt32(channels.Count);
            foreach (Channel channel in channels)
            {
                b.WriteInt16(channel.Tab);
                b.WriteInt16(channel.Number);
                b.WriteInt32(channel.MaxLoad);
                b.WriteInt32(channel.CurrentLoad);
            }

            BafPacket p = new BafPacket(PacketId.ChannelListRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}