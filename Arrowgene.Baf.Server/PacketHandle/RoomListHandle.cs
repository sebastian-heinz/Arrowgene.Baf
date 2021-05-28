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
            int numRooms = 10;

            IBuffer b = new StreamBuffer();
            b.WriteInt32(numRooms); // num of rooms
            b.WriteByte(0);
            for (int i = 0; i < numRooms; i++)
            {
                b.WriteInt32(i);
                b.WriteInt32(1); //0 = invisible | 1 = wait | 2 = playing
                b.WriteCString("Room: " + i);
                b.WriteByte(0); // has password
                b.WriteByte((byte) TeamType.Team);
                b.WriteByte((byte) KeyType.Key7);
                b.WriteByte(1); // allow spectator
                b.WriteInt32(0); // battery
                b.WriteByte(0); // unknown 
                b.WriteByte(6); // slots available
                int numPlayer = 1;
                b.WriteByte((byte) numPlayer); // slots occupied 
                for (int j = 0; j < numPlayer; j++)
                {
                    b.WriteInt32(j);// playerId ?
                }
                b.WriteByte(1); // unknown
                int numUnk = 1;
                b.WriteByte((byte) numUnk); // perhaps num viewer ?
                for (int j = 0; j < numUnk; j++)
                {
                    b.WriteInt32(j);// playerId ?
                }
            }

            BafPacket p = new BafPacket(PacketId.RoomListRes, b.GetAllBytes());
            client.Send(p);
        }

    }
}