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
            byte unk = buffer.ReadByte(); // ??? 00 = Single, 01 = Team ; 03 = Band Ensemble ???
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
            
            IBuffer b = new StreamBuffer();

            b.WriteInt16(6); // room number 0 - 299 (001 - 300)
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0); // 0 = yellow | 1 = purple | 3 = blue
            b.WriteByte((byte)keyMode);
            b.WriteByte(hasPassword ? (byte)1 : (byte)0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(10); // battery
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