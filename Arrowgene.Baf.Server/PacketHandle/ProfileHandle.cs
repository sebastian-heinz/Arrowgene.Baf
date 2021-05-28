using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class ProfileHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(ProfileHandle));

        public override PacketId Id => PacketId.ProfileReq;

        public ProfileHandle(BafServer server) : base(server)
        {
        }
        
        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer b = new StreamBuffer();
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteCString(client.Character.Name);
            b.WriteByte(0);
            b.WriteInt32(99); // level
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteCString("TITLE");
            b.WriteCString("SELF INTRODUCTION");
            b.WriteByte(0);
            b.WriteInt32(20); // friends
            b.WriteCString("AGE");
            b.WriteCString("START SIGN");            
            b.WriteCString("CITY");
            b.WriteCString("DISCORD");
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
            b.WriteInt32(0);
   
            BafPacket p = new BafPacket(PacketId.ProfileRes, b.GetAllBytes());
               client.Send(p);
        }

    }
}