using System;
using System.Threading;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class RoomGameLoadingHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(RoomGameLoadingHandle));

        public override PacketId Id => PacketId.RoomGameLoadingHandleReq;

        public RoomGameLoadingHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
           // DateTime foo = DateTime.Now;
           // long unixTime = ((DateTimeOffset) foo).ToUnixTimeSeconds();
           // long ms = unixTime / 1000;
           // int ims = (int) ms;
           
            IBuffer b = new StreamBuffer();
            b.WriteInt64(0); // read 8bytes
            BafPacket p = new BafPacket(PacketId.RoomGameLoadingFinishedRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}