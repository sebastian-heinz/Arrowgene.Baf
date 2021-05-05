using System;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Buffers;
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

        private BafClient InitializeNewClient(ITcpSocket socket, byte[] data)
        {
            if (data.Length != 40)
            {
                // expected 40 byte init packet
                return null;
            }

            IBuffer recv = new StreamBuffer(data);
            //   PacketFactory pf = new PacketFactory();

            //   IBuffer recv = new StreamBuffer(data);
            //   recv.SetPositionStart();
            //   ushort sz = recv.ReadUInt16();
            //   byte[] rcv = recv.ReadBytes(sz - 2);

            //   Logger.Debug(Environment.NewLine + Util.HexDump(rcv));
            //   byte[] packet = BafXor.Xor_1(rcv);
            //   Logger.Debug(Environment.NewLine + Util.HexDump(packet));

            //   IBuffer pBuf = new StreamBuffer(packet);
            //   pBuf.SetPositionStart();
            //   byte[] a = pBuf.ReadBytes(16);
            //   byte[] b = pBuf.ReadBytes(8);
            //   byte[] c = pBuf.ReadBytes(8);
            //   byte[] d = pBuf.ReadBytes(8);
            //   
            //   using var md5 = MD5.Create();
            //   byte[] hash = a;
            //   for (int i = 0; i < 0x10; i++)
            //   {
            //       hash = md5.ComputeHash(hash);
            //   }
            //   IBuffer phash = new StreamBuffer(hash);
            //   phash.SetPositionStart();
            //   byte[] e = phash.ReadBytes(8);
            //   byte[] f = phash.ReadBytes(8);
            //   
            //   byte[] dec = Dec(b, f, e);
            //         Logger.Debug(Environment.NewLine + Util.HexDump(dec));
            return null;
        }

        protected override void HandleReceived(ITcpSocket socket, byte[] data)
        {
            if (!socket.IsAlive)
            {
                return;
            }

            BafClient client;
            if (_clients[socket.UnitOfOrder].ContainsKey(socket))
            {
                client = _clients[socket.UnitOfOrder][socket];
            }
            else
            {
                client = InitializeNewClient(socket, data);
                if (client == null)
                {
                    // failed
                    return;
                }

                _clients[socket.UnitOfOrder].Add(socket, client);
            }

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
        }

        protected override void HandleDisconnected(ITcpSocket socket)
        {
            Logger.Debug("HandleDisconnected");
        }

        protected override void HandleConnected(ITcpSocket socket)
        {
            Logger.Debug("HandleConnected");
        }
    }
}