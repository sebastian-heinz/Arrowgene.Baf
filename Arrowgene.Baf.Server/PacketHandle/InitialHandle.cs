using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class InitialHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(InitialHandle));

        public override PacketId Id => PacketId.InitialReq;

        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();
            BafPacket p = new BafPacket(PacketId.InitialRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}