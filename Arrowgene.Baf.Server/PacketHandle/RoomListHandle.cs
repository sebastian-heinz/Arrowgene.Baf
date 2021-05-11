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

        public override void Handle(BafClient client, BafPacket packet)
        {
            int numRooms = 3;
            
            IBuffer b = new StreamBuffer();
            b.WriteInt32(numRooms); // num of rooms
            b.WriteByte(0);
            for (int i = 0; i < numRooms; i++)
            {
                b.WriteInt32(i);
                b.WriteByte(1); //0 = invisible | 1 = join | 2 = playing | //perhaps number of player & status
                b.WriteByte(0); 
                b.WriteByte(0);
                b.WriteByte(0);
                b.WriteCString("Room: " + i);
                b.WriteByte(0); // has password
                b.WriteByte((byte)TeamType.Single);
                b.WriteByte((byte)KeyType.Key7);
                b.WriteByte(0); // allow spectator
                b.WriteByte(5);
                b.WriteByte(0);
                b.WriteByte(0);
                b.WriteByte(0);
                b.WriteByte(0); //req longer packet ?
                b.WriteByte(4); // slots available
                b.WriteByte(0); // req longer packet?
                b.WriteByte(0);
                b.WriteByte(0);
            }
            b.WriteByte(0);

            BafPacket p = new BafPacket(PacketId.RoomListRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}