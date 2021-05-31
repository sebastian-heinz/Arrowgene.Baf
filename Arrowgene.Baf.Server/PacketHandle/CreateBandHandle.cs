using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class CreateBandHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(CreateBandHandle));

        public override PacketId Id => PacketId.CreateBandReq;

        public CreateBandHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            int unk = buffer.ReadInt32();
            string bandName = buffer.ReadCString();
            byte unk1 = buffer.ReadByte();
            Logger.Info($"Create Band:{bandName}");
            
            IBuffer b = new StreamBuffer();
            BafPacket p = new BafPacket(PacketId.CreateBandRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}