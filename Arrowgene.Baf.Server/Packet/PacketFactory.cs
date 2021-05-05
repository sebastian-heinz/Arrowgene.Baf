using System;
using System.Collections.Generic;
using System.Text;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.Packet
{
    /**
     * [16 bytes] key seed
     * [8  bytes] encrypted end block
     * [4  bytes] size of packet data without padding
     * [4  bytes] size of packet data including padding
     * [x  bytes] packet data + padding bytes (0x4D)
     */
    public class PacketFactory
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(PacketFactory));

        private const int PacketLengthSize = 2;
        private const int PacketIdSize = 2;

        private IBuffer _buffer;
        private ushort _dataSize;
        private int _position;
        private bool _readPacketLength;
        private readonly BafXor.Stateful _xor;

        public PacketFactory()
        {
            _xor = BafXor.CreateStatefulPacket();
            Reset();
        }

        public byte[] Write(BafPacket packet)
        {
            int packetSize = packet.Data.Length + PacketLengthSize + PacketIdSize;
            if (packetSize > ushort.MaxValue)
            {
                return null;
            }

            if (packetSize < 0)
            {
                return null;
            }

            ushort size = (ushort) packetSize;

            IBuffer buffer = new StreamBuffer();
            buffer.WriteUInt16(size);
            buffer.WriteUInt16(packet.Id);
            buffer.WriteBytes(packet.Data);
            byte[] packetData = buffer.GetAllBytes();

            // TODO apply crypto?
            // BafXor.Xor(packetData);
            Logger.Debug($"Send:{Environment.NewLine}{Util.HexDump(packetData)}");

            return packetData;
        }

        public List<BafPacket> Read(byte[] data)
        {
            List<BafPacket> packets = new List<BafPacket>();
            if (_buffer == null)
            {
                _buffer = new StreamBuffer(data);
            }
            else
            {
                _buffer.SetPositionEnd();
                _buffer.WriteBytes(data);
            }

            _buffer.Position = _position;

            bool read = true;
            while (read)
            {
                read = false;
                if (!_readPacketLength && _buffer.Size - _buffer.Position > PacketLengthSize)
                {
                    ushort dataSize = _buffer.ReadUInt16();
                    int iDataSize = dataSize - PacketLengthSize;
                    if (iDataSize < 0)
                    {
                        // error
                    }

                    if (iDataSize > ushort.MaxValue)
                    {
                        // error
                    }

                    _dataSize = (ushort) iDataSize;
                    _readPacketLength = true;
                }

                if (_readPacketLength && _buffer.Size - _buffer.Position >= _dataSize)
                {
                    StringBuilder packetLog = new StringBuilder();

                    byte[] packetData = _buffer.ReadBytes(_dataSize);
                    packetLog.Insert(0, $"Raw:{Environment.NewLine}{Util.HexDump(packetData)}");
                    _xor.StatefulXor(packetData);
                    packetLog.Insert(0, $"Xor:{Environment.NewLine}{Util.HexDump(packetData)}");

                    IBuffer buffer = new StreamBuffer(packetData);
                    buffer.SetPositionStart();

                    byte[] password = buffer.ReadBytes(16);
                    byte[] endBlock = buffer.ReadBytes(8);
                    uint packetSize = buffer.ReadUInt32();
                    uint totalSize = buffer.ReadUInt32();
                    BafPbeWithMd5AndDes.DesKey key = BafPbeWithMd5AndDes.DeriveKey(password);
                    int remaining = buffer.Size - buffer.Position;
                    byte[] encrypted = buffer.ReadBytes(remaining);

                    byte[] decrypted = BafPbeWithMd5AndDes.Decrypt(encrypted, key);
                    packetLog.Insert(0, $"Dec:{Environment.NewLine}{Util.HexDump(decrypted)}");

                    if (decrypted.Length != totalSize)
                    {
                        // err: unexpected size
                    }

                    IBuffer decryptedBuffer = new StreamBuffer(decrypted);
                    decryptedBuffer.SetPositionStart();
                    ushort packetId = decryptedBuffer.ReadUInt16();
                    packetLog.Insert(0,
                        $"Received:{Environment.NewLine}[PacketId: {packetId}] [packetSize: {packetSize}] [totalSize: {totalSize}] [endBlock: {Util.ToHexString(endBlock, ' ')}]{Environment.NewLine}");
                    Logger.Debug(packetLog.ToString());

                    int payloadSize = (int) packetSize - PacketIdSize;
                    if (payloadSize > uint.MaxValue)
                    {
                        // err
                    }

                    if (payloadSize < 0)
                    {
                        // err
                    }

                    int decryptedRemaining = decryptedBuffer.Size - decryptedBuffer.Position;

                    if (payloadSize > decryptedRemaining)
                    {
                        // err
                    }

                    byte[] payload = decryptedBuffer.ReadBytes(payloadSize);
                    BafPacket packet = new BafPacket(packetId, payload);
                    packets.Add(packet);

                    _readPacketLength = false;
                    read = _buffer.Position != _buffer.Size;
                }
            }

            if (_buffer.Position == _buffer.Size)
            {
                // TODO reuse buffer, avoid new allocation
                Reset();
            }
            else
            {
                _position = _buffer.Position;
            }

            return packets;
        }

        private void Reset()
        {
            _readPacketLength = false;
            _dataSize = 0;
            _position = 0;
            _buffer = null;
        }
    }
}