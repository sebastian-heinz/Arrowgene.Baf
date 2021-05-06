using System.Globalization;
using System.Text;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class ChannelListHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(ChannelListHandle));

        public override PacketId Id => PacketId.ChannelListReq; 

        public override void Handle(BafClient client, BafPacket packet)
        {
           
            IBuffer b = new StreamBuffer();
            b.WriteInt32(1); // number of channels
            // channel start
            b.WriteInt16(0); // tab 100, 200, 300
            b.WriteInt16(0); // number 1XX
            
            b.WriteInt32(10); // max load
            b.WriteInt32(5); // current load
            
      
            b.WriteString("aaaaa");
            b.WriteString("aaaaa");
            b.WriteString("aaaaa");
            b.WriteByte(2);
            b.WriteString("abbbbbbbbb");
            b.WriteByte(2);
            b.WriteString("ccccccc");
            b.WriteByte(2);
            b.WriteString("a");
            b.WriteByte(2);
            b.WriteString("a");
            b.WriteByte(2);
            b.WriteString("a");
            b.WriteByte(255);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(0);
            b.WriteByte(123);
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
            
            
            BafPacket p = new BafPacket(PacketId.ChannelListRes,  b.GetAllBytes());
            client.Send(p);
            
   
            
        }

    }
}