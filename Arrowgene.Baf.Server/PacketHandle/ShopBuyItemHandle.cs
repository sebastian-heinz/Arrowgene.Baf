using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
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
            int itemId = buffer.ReadInt32();
            Logger.Info($"Purchase Item: Id:{itemId}");
            
            IBuffer b = new StreamBuffer();

            BafPacket p = new BafPacket(PacketId.ShopBuyItemRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}