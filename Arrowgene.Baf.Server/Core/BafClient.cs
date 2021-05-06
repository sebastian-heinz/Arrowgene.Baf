using System;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp;

namespace Arrowgene.Baf.Server.Core
{
    public class BafClient
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(BafClient));

        private readonly ITcpSocket _socket;
        private readonly PacketFactory _packetFactory;

        public BafClient(ITcpSocket clientSocket)
        {
            _socket = clientSocket;
            _packetFactory = new PacketFactory(this);
            Identity = _socket.Identity;
        }

        public string Identity { get; }

        public List<BafPacket> Receive(byte[] data)
        {
            List<BafPacket> packets;
            try
            {
                packets = _packetFactory.Read(data);
            }
            catch (Exception ex)
            {
                Logger.Exception(this, ex);
                packets = new List<BafPacket>();
            }

            return packets;
        }

        public void Send(BafPacket packet)
        {
            byte[] data;
            try
            {
                data = _packetFactory.Write(packet);
            }
            catch (Exception ex)
            {
                Logger.Exception(this, ex);
                return;
            }

            if (data == null)
            {
                Logger.Error(this, $"No data produced to send for packetId: {packet.Id}");
                return;
            }

            _socket.Send(data);
        }
    }
}