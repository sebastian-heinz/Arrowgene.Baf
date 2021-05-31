using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class Unknown0Handle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(Unknown0Handle));

        public override PacketId Id => PacketId.Unknown2Req;

        public Unknown0Handle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();
            b.WriteInt32(1);
            b.WriteInt32(2);
            b.WriteInt32(3);
            b.WriteInt32(4);
            b.WriteInt32(5);
            b.WriteInt32(6);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            
            BafPacket p = new BafPacket(PacketId.Unknown2Res, b.GetAllBytes());
           // client.Send(p);
        }

    }
}