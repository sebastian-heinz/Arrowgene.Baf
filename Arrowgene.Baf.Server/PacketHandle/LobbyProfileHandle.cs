using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class LobbyProfileHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(LobbyProfileHandle));

        public override PacketId Id => PacketId.LobbyProfileReq;

        public LobbyProfileHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            Character character = client.Character;
            if (character == null)
            {
                Logger.Error(client, "Character is null");
                return;
            }
            
            
            IBuffer b = new StreamBuffer();

            b.WriteInt32(1);
            b.WriteCString(character.Name);
            b.WriteByte((byte)character.Gender);
            b.WriteInt32(character.GCoins);
            b.WriteInt32(character.MCoins);
            b.WriteInt32(character.Level);
            b.WriteInt32(20); // cog Music
            b.WriteInt32(30);
            b.WriteInt32(40);
            b.WriteInt32(50);
            b.WriteByte(0);
            b.WriteInt32(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteInt32(character.GetEquippedItemId(ItemType.Head));
            b.WriteInt32(character.GetEquippedItemId(ItemType.Hair));
            b.WriteInt32(character.GetEquippedItemId(ItemType.Chest));
            b.WriteInt32(character.GetEquippedItemId(ItemType.Pants));
            b.WriteInt32(character.GetEquippedItemId(ItemType.Hands));
            b.WriteInt32(character.GetEquippedItemId(ItemType.Shoes));
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