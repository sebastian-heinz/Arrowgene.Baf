using System;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp;
using Arrowgene.Networking.Tcp.Consumer.BlockingQueueConsumption;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.Baf.Server.Core
{
    public class BafQueueConsumer : ThreadedBlockingQueueConsumer
    {
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(BafQueueConsumer));

        private readonly Dictionary<PacketId, IPacketHandler> _packetHandlers;
        private readonly Dictionary<ITcpSocket, BafClient>[] _clients;

        public BafQueueConsumer(AsyncEventSettings socketSetting) : base(socketSetting, "BafQueueConsumer")
        {
            _clients = new Dictionary<ITcpSocket, BafClient>[socketSetting.MaxUnitOfOrder];
            _packetHandlers = new Dictionary<PacketId, IPacketHandler>();
        }

        public int GetHandlerCount()
        {
            return _packetHandlers.Count;
        }

        public void ClearHandler()
        {
            _packetHandlers.Clear();
        }

        public void AddHandler(IPacketHandler packetHandler, bool overwrite = false)
        {
            if (_packetHandlers.ContainsKey(packetHandler.Id))
            {
                if (overwrite)
                {
                    _packetHandlers[packetHandler.Id] = packetHandler;
                    Logger.Info($"Packet Handler: {packetHandler.Id} got reassigned");
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
                    Logger.Error(client, $"HandleReceived: no packet handler registered for: {packet.Id}");
                    continue;
                }

                IPacketHandler packetHandler = _packetHandlers[packet.Id];
                try
                {
                    packetHandler.Handle(client, packet);
                }
                catch (Exception ex)
                {
                    Logger.Exception(client, ex);
                }
            }
        }

        protected override void HandleDisconnected(ITcpSocket socket)
        {
            if (!_clients[socket.UnitOfOrder].ContainsKey(socket))
            {
                Logger.Error(socket,"HandleDisconnected: client not found");
                return;
            }

            BafClient client = _clients[socket.UnitOfOrder][socket];
            _clients[socket.UnitOfOrder].Remove(socket);
            Logger.Info(client, "Disconnected");
        }

        protected override void HandleConnected(ITcpSocket socket)
        {
            if (_clients[socket.UnitOfOrder].ContainsKey(socket))
            {
                Logger.Error(socket,"HandleConnected: client already connected");
                return;
            }

            BafClient client = new BafClient(socket);
            _clients[socket.UnitOfOrder].Add(socket, client);
            Logger.Info(client, "Connected");
        }
    }
}