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
            byte pTeamType = buffer.ReadByte();
            byte pKeyType = buffer.ReadByte();
            byte pAllowSpectators = buffer.ReadByte();
            bool allowSpectators = pAllowSpectators == 1;
            byte pHasPassword = buffer.ReadByte();
            bool hasPassword = pHasPassword == 1;
            string password = "";
            if (hasPassword)
            {
                 password = buffer.ReadCString();
            }
            uint unk4 = buffer.ReadUInt32();
            TeamType team = (TeamType)pTeamType;
            KeyType key = (KeyType)pKeyType;
            
            Logger.Debug($"Create Room: Name:{name} Pw:{password} Key:{key} Team:{team} Spectators:{allowSpectators}");
            Logger.Debug($"unk4:{unk4}");

            IBuffer b = new StreamBuffer();
            b.WriteInt16(1); // room number 0 - 299 (001 - 300)
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteCString(name);
            b.WriteByte((byte)team);
            b.WriteByte((byte)key);
            b.WriteByte(hasPassword ? (byte)1 : (byte)0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(allowSpectators? (byte)1 : (byte)0); // spectator 0=false 1 = true
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(70); // battery
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);

            BafPacket p = new BafPacket(PacketId.CreateRoomRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}