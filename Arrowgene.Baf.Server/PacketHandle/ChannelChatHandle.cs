using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class ChannelChatHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(ChannelChatHandle));

        public override PacketId Id => PacketId.ChannelChatReq;

        public ChannelChatHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            string message = buffer.ReadCString();
            Logger.Info(client, $"ChannelChat Message: {message}");
            
            Character character = client.Character;
            if (character == null)
            {
                Logger.Error(client, "Character is null");
                return;
            }
            
            IBuffer b = new StreamBuffer();
            b.WriteCString(character.Name);
            b.WriteCString(message);
            BafPacket p = new BafPacket(PacketId.ChannelChatRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}