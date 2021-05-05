using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class Crypt_F303 : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(Crypt_F303));
        
        public override ushort Id => 0x3F3; // 1011
        public override void Handle(BafClient client, BafPacket packet)
        {
            Logger.Debug("Crypt_F303");
            IBuffer b = new StreamBuffer();
            BafPacket p = new BafPacket(1012, b.GetAllBytes());
            client.Send(p);
        }
    }
}