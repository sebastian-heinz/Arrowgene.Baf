using System;
using System.Collections;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
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
        private static readonly BafLogger Logger = LogProvider.Logger<BafLogger>(typeof(PacketFactory));

        private const int PacketLengthSize = 2;
        private const int PacketIdSize = 2;
        private const int PacketHeaderSize = 32;

        private IBuffer _buffer;
        private ushort _dataSize;
        private int _position;
        private bool _readPacketLength;
        private readonly BafXor.Stateful _xor;
        private readonly BafClient _client;

        public PacketFactory(BafClient client)
        {
            _client = client;
            _xor = BafXor.CreateStatefulPacket();
            Reset();
        }

        public byte[] Write(BafPacket packet)
        {
            Logger.Packet(_client, packet);
            int packetSize = packet.Data.Length + PacketLengthSize + PacketIdSize;
            if (packetSize > ushort.MaxValue)
            {
                Logger.Error(_client, $"Write: packetSize({packetSize}) > ushort.MaxValue({ushort.MaxValue})");
                return null;
            }

            if (packetSize < 0)
            {
                Logger.Error(_client, $"Write: packetSize({packetSize}) < 0");
                return null;
            }

            ushort packetId = (ushort) packet.Id;
            if (packet.Id == PacketId.Unknown)
            {
                // packet crafted without enum id
                if (packet.IdValue == packetId)
                {
                    Logger.Error(_client, $"Write: tried to send invalid packet");
                    return null;
                }

                packetId = packet.IdValue;
            }

            ushort size = (ushort) packetSize;
            IBuffer buffer = new StreamBuffer();
            buffer.WriteUInt16(size);
            buffer.WriteUInt16(packetId);
            buffer.WriteBytes(packet.Data);
            byte[] packetData = buffer.GetAllBytes();
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
                        Logger.Error(_client, $"Read: iDataSize({iDataSize}) < 0");
                        Reset();
                        return packets;
                    }

                    if (iDataSize > ushort.MaxValue)
                    {
                        Logger.Error(_client, $"Read: iDataSize({iDataSize}) > ushort.MaxValue({ushort.MaxValue})");
                        Reset();
                        return packets;
                    }

                    _dataSize = (ushort) iDataSize;
                    _readPacketLength = true;
                }

                if (_readPacketLength && _buffer.Size - _buffer.Position >= _dataSize)
                {
                    if (_dataSize < PacketHeaderSize)
                    {
                        Logger.Error(_client, $"Read: _dataSize({_dataSize}) < PacketHeaderSize({PacketHeaderSize})");
                        Reset();
                        return packets;
                    }

                    byte[] packetData = _buffer.ReadBytes(_dataSize);
                    _xor.StatefulXor(packetData);

                    IBuffer buffer = new StreamBuffer(packetData);
                    buffer.SetPositionStart();
                    byte[] password = buffer.ReadBytes(16);
                    byte[] expectedEndBlock = buffer.ReadBytes(8);
                    uint bodySize = buffer.ReadUInt32();
                    uint expectedBodySizeWithPadding = buffer.ReadUInt32();

                    int bodySizeWithPadding = buffer.Size - buffer.Position;
                    if (bodySizeWithPadding < 0)
                    {
                        Logger.Error(_client,
                            $"Read: bodySizeWithPadding({expectedBodySizeWithPadding}) < 0");
                        Reset();
                        return packets;
                    }

                    if (expectedBodySizeWithPadding != bodySizeWithPadding)
                    {
                        Logger.Error(_client,
                            $"Read: expectedBodySizeWithPadding({expectedBodySizeWithPadding}) != bodySizeWithPadding({bodySizeWithPadding})");
                        Reset();
                        return packets;
                    }

                    if (bodySize > bodySizeWithPadding)
                    {
                        Logger.Error(_client,
                            $"Read: bodySize({bodySize}) > expectedBodySizeWithPadding({expectedBodySizeWithPadding})");
                        Reset();
                        return packets;
                    }

                    int endBlockOffset = PacketHeaderSize + bodySizeWithPadding - expectedEndBlock.Length;
                    byte[] endBlock = buffer.GetBytes(endBlockOffset, expectedEndBlock.Length);
                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(expectedEndBlock, endBlock))
                    {
                        Logger.Error(_client,
                            $"Read: !StructuralComparisons.StructuralEqualityComparer.Equals(expectedEndBlock({Util.ToHexString(expectedEndBlock, ' ')}), endBlock({Util.ToHexString(endBlock, ' ')})");
                        Reset();
                        return packets;
                    }

                    long payloadSize = bodySize - PacketIdSize;
                    if (payloadSize > uint.MaxValue)
                    {
                        Logger.Error(_client, $"Read: payloadSize({payloadSize}) > uint.MaxValue({uint.MaxValue})");
                        Reset();
                        return packets;
                    }

                    if (payloadSize < 0)
                    {
                        Logger.Error(_client, $"Read: payloadSize({payloadSize}) < 0");
                        Reset();
                        return packets;
                    }

                    // TODO support ulong/long size in buffer.ReadBytes()
                    if (payloadSize > int.MaxValue)
                    {
                        Logger.Error(_client,
                            $"Read: payloadSize({payloadSize}) > int.MaxValue({int.MaxValue})  (data larger than int.Max not supported at the moment)");
                        Reset();
                        return packets;
                    }

                    if (bodySize > int.MaxValue)
                    {
                        Logger.Error(_client,
                            $"Read: bodySize({bodySize}) > int.MaxValue({int.MaxValue})  (data larger than int.Max not supported at the moment)");
                        Reset();
                        return packets;
                    }

                    int dataSize = (int) payloadSize;

                    BafPbeWithMd5AndDes.DesKey key = BafPbeWithMd5AndDes.DeriveKey(password);
                    byte[] encrypted = buffer.ReadBytes(bodySizeWithPadding);
                    byte[] decrypted = BafPbeWithMd5AndDes.Decrypt(encrypted, key);

                    IBuffer decryptedBuffer = new StreamBuffer(decrypted);
                    decryptedBuffer.SetPositionStart();
                    ushort packetId = decryptedBuffer.ReadUInt16();
                    byte[] payload = decryptedBuffer.ReadBytes(dataSize);

                    BafPacket packet = new BafPacket(packetId, payload, PacketSource.Client);
                    Logger.Packet(_client, packet);
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