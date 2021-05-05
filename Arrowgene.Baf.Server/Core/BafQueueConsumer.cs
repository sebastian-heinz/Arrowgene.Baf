using System;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp;
using Arrowgene.Networking.Tcp.Consumer.BlockingQueueConsumption;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.Baf.Server.Core
{
    public class BafQueueConsumer : ThreadedBlockingQueueConsumer
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(BafQueueConsumer));

        private readonly Dictionary<ushort, IPacketHandler> _packetHandlers;
        private readonly Dictionary<ITcpSocket, BafClient>[] _clients;

        public BafQueueConsumer(AsyncEventSettings socketSetting) : base(socketSetting, "BafQueueConsumer")
        {
            _clients = new Dictionary<ITcpSocket, BafClient>[socketSetting.MaxUnitOfOrder];
            _packetHandlers = new Dictionary<ushort, IPacketHandler>();
        }

        public void AddHandler(IPacketHandler packetHandler, bool overwrite = false)
        {
            if (_packetHandlers.ContainsKey(packetHandler.Id))
            {
                if (overwrite)
                {
                    _packetHandlers[packetHandler.Id] = packetHandler;
                }

                return;
            }

            _packetHandlers.Add(packetHandler.Id, packetHandler);
        }

        public override void OnStart()
        {
            base.OnStart();
            for (int i = 0; i < _clients.Length; i++)
            {
                _clients[i] = new Dictionary<ITcpSocket, BafClient>();
            }
        }

        protected override void HandleReceived(ITcpSocket socket, byte[] data)
        {
            if (!socket.IsAlive)
            {
                return;
            }
            
            if (!_clients[socket.UnitOfOrder].ContainsKey(socket))
            {
                return;
            }

            BafClient client = _clients[socket.UnitOfOrder][socket];
            List<BafPacket> packets = client.Receive(data);
            foreach (BafPacket packet in packets)
            {
                if (!_packetHandlers.ContainsKey(packet.Id))
                {
                    continue;
                }

                IPacketHandler packetHandler = _packetHandlers[packet.Id];
                try
                {
                    packetHandler.Handle(client, packet);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
            Logger.Debug("HandleReceived");
        }

        protected override void HandleDisconnected(ITcpSocket socket)
        {
            if (!_clients[socket.UnitOfOrder].ContainsKey(socket))
            {
                Logger.Error("Socket already removed");
                return;
            }

            _clients[socket.UnitOfOrder].Remove(socket);
            Logger.Debug("HandleDisconnected");
        }

        protected override void HandleConnected(ITcpSocket socket)
        {
            if (_clients[socket.UnitOfOrder].ContainsKey(socket))
            {
                Logger.Error("Socket already connected");
                return;
            }
            BafClient client = new BafClient(socket);
            _clients[socket.UnitOfOrder].Add(socket, client);
            Logger.Debug("HandleConnected");
        }
    }
}