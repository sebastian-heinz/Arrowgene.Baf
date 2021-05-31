using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class RoomSelectSongHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(RoomSelectSongHandle));

        public override PacketId Id => PacketId.RoomSelectSongReq;

        public RoomSelectSongHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
            IBuffer buffer = packet.CreateBuffer();
            int songId = buffer.ReadInt32();
            byte difficulty = buffer.ReadByte();
            SongDifficultyType songDifficulty = (SongDifficultyType) difficulty;
            Logger.Info(client, $"Selected Song: Id:{songId} Difficulty:{songDifficulty}");

            IBuffer b = new StreamBuffer();
            b.WriteInt32(songId);
            BafPacket p = new BafPacket(PacketId.RoomSelectSongRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}