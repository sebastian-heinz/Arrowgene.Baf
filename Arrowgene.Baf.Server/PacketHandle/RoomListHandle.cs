using System.Collections.Generic;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class RoomListHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(RoomListHandle));

        public override PacketId Id => PacketId.RoomListReq;

        public RoomListHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
            List<Room> rooms = client.Channel.GetRooms();

            IBuffer b = new StreamBuffer();
            b.WriteInt32(rooms.Count); // num of rooms
            b.WriteByte(0);
            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];
                b.WriteInt32(room.Id);
                b.WriteInt32(1); //0 = invisible | 1 = wait | 2 = playing
                b.WriteCString(room.Name);
                b.WriteByte(room.HasPassword ? (byte) 1 : (byte) 0);
                b.WriteByte((byte) room.Team);
                b.WriteByte((byte) room.Key);
                b.WriteByte(room.AllowSpectators ? (byte) 1 : (byte) 0);
                b.WriteInt32(0); // battery
                b.WriteByte(0); // unknown 
                b.WriteByte(6); // slots available
                int numPlayer = 1;
                b.WriteByte((byte) numPlayer); // slots occupied 
                for (int j = 0; j < numPlayer; j++)
                {
                    b.WriteInt32(j); // playerId ?
                }

                b.WriteByte(1); // unknown
                int numUnk = 1;
                b.WriteByte((byte) numUnk); // perhaps num viewer ?
                for (int j = 0; j < numUnk; j++)
                {
                    b.WriteInt32(j); // playerId ?
                }
            }

            BafPacket p = new BafPacket(PacketId.RoomListRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}