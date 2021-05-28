using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.PacketHandle
{
    public class LoginHandle : PacketHandler
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(LoginHandle));

        public override PacketId Id => PacketId.LoginReq;

        public LoginHandle(BafServer server) : base(server)
        {
        }

        public override void Handle(BafClient client, BafPacket packet)
        {
            byte[] data = packet.Data;
            // Logger.Data(client, data, "Login");
            // BafXor.XorLogin(data);
            // Logger.Data(client, data, "LoginXor");

            IBuffer buffer = new StreamBuffer(data);
            buffer.SetPositionStart();
            string account = buffer.ReadCString();
            string password = buffer.ReadCString();
            string pin = buffer.ReadCString();

            Logger.Debug($"Login: Acc:{account} Pw:{password} Pin:{pin}");

            Character character = new Character();
            character.Name = account;
            client.Character = character;

            IBuffer b = new StreamBuffer();
            b.WriteInt32(0);

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


            BafPacket p = new BafPacket(PacketId.LoginRes, b.GetAllBytes());
            client.Send(p);
        }
    }
}