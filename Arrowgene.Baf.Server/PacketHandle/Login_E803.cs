using Arrowgene.Baf.Server.Common;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class Login_E803 : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(Login_E803));
        
        public override ushort Id => 0x3e8; // 1011
        public override void Handle(BafClient client, BafPacket packet)
        {
            Logger.Debug("Login_E803");

            byte[] data = packet.Data;
            BafXor.XorLogin(data);
            IBuffer buffer = new StreamBuffer(data);
            buffer.SetPositionStart();
            string account = buffer.ReadCString();
            string password = buffer.ReadCString();
            string pin = buffer.ReadCString();

            Logger.Debug($"Acc:{account} Pw:{password} Pin:{pin}");
        }
    }
}