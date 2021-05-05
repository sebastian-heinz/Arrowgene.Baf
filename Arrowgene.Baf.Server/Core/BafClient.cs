using System;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp;

namespace Arrowgene.Baf.Server.Core
{
    public class BafClient
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(BafClient));

        public BafClient(ITcpSocket clientSocket, PacketFactory packetFactory)
        {
            _socket = clientSocket;
            _packetFactory = packetFactory;
        }

        private readonly ITcpSocket _socket;
        private readonly PacketFactory _packetFactory;

        public List<BafPacket> Receive(byte[] data)
        {
            List<BafPacket> packets;
            try
            {
                packets = _packetFactory.Read(data);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
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
                Logger.Exception(ex);
                return;
            }

            _socket.Send(data);
        }
    }
}