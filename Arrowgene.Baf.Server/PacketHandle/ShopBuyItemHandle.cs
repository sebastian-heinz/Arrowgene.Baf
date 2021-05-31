using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class ShopBuyItemHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(ShopBuyItemHandle));

        public override PacketId Id => PacketId.ShopBuyItemReq;

        public ShopBuyItemHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            int shopItemId = buffer.ReadInt32();
            Logger.Info(client, $"Purchase ShopItemId: {shopItemId}");

            ShopItem shopItem = _server.GetShopItem(shopItemId);
            if (shopItem == null)
            {
                Logger.Error(client, $"Invalid ShopItemId: {shopItemId}");
                return;
            }

            Character character = client.Character;
            if (character == null)
            {
                Logger.Error(client, $"Character is null");
                return;
            }
            
            

            IBuffer b = new StreamBuffer();

            BafPacket p = new BafPacket(PacketId.ShopBuyItemRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}