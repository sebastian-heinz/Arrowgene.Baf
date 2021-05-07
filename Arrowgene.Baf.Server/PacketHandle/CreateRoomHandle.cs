using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class CreateRoomHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(CreateRoomHandle));

        public override PacketId Id => PacketId.CreateRoomReq;

        public override void Handle(BafClient client, BafPacket packet)
        {

            IBuffer buffer = packet.CreateBuffer();
            string name = buffer.ReadCString();
            byte unk = buffer.ReadByte();
            byte pKeyMode = buffer.ReadByte();
            KeyModeType keyMode = (KeyModeType)pKeyMode;
            byte unk2 = buffer.ReadByte();
            byte pHasPassword = buffer.ReadByte();
            bool hasPassword = pHasPassword == 1;
            string password = "";
            if (hasPassword)
            {
                 password = buffer.ReadCString();
            }
            uint unk4 = buffer.ReadUInt32();
            Logger.Debug($"Create Room: Name:{name} Pw:{password} KeyMode:{keyMode}");

            // 00000000   B5 C4 B7 BF BC E4 00 00  00 01 00 00 00 00 00      µÄ·¿¼ä········· 
            // 00000000   B5 C4 B7 BF BC E4 00 01  00 01 00 01 00 00 00      µÄ·¿¼ä········· 
            // 00000000   B5 C4 B7 BF BC E4 00 01  00 01 00 02 00 00 00      µÄ·¿¼ä········· 
            // 00000000   B5 C4 B7 BF BC E4 00 03  00 01 00 01 00 00 00      µÄ·¿¼ä········· 


            IBuffer b = new StreamBuffer();

            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            
            b.WriteByte(3);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            
            b.WriteByte(0);
            b.WriteByte(1);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0x25);
            b.WriteByte(0);
            b.WriteByte(0xD5);
            b.WriteByte(7);
            b.WriteByte(3);

            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0x78);
            b.WriteByte(0x61);
            b.WriteByte(0x2D);
            b.WriteByte(0x6D);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0xFB);
            b.WriteByte(0xD);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);

            BafPacket p = new BafPacket(PacketId.CreateRoomRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}