using System.Collections;
using System.Collections.Generic;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.Packet
{
    public class PacketFactory
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(PacketFactory));

        private const int PacketLengthSize = 2;

        private IBuffer _buffer;
        private ushort _dataSize;
        private int _position;
        private bool _readPacketLength;
        private BafPbeWithMd5AndDes.DesKey _key;

        public PacketFactory()
        {
            _key = null;
            Reset();
        }

        public byte[] Write(BafPacket packet)
        {
            return null;
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
                    byte[] packetData = _buffer.ReadBytes(_dataSize);
                    BafXor.Xor(packetData);
                    if (_key == null)
                    {
                        // No key received yet, expect key payload and validate
                        if (_dataSize != 40)
                        {
                            Logger.Error("expected 40 bytes");
                        }
                        IBuffer keyBuffer = new StreamBuffer(packetData);
                        keyBuffer.SetPositionStart();
                        byte[] password = keyBuffer.ReadBytes(16);
                        byte[] payloadA = keyBuffer.ReadBytes(8);
                        uint a = keyBuffer.ReadUInt32();
                        uint b = keyBuffer.ReadUInt32();
                        byte[] payloadB = keyBuffer.ReadBytes(8);
                        if (a != 2)
                        {
                            Logger.Error("expected 2");
                        }

                        if (b != 8)
                        {
                            Logger.Error("expected 8");
                        }

                        if (!StructuralComparisons.StructuralEqualityComparer.Equals(payloadA, payloadB))
                        {
                            Logger.Error("payloadA == payloadB");
                        }

                        _key = BafPbeWithMd5AndDes.DeriveKey(password, 16);

                        packetData = payloadA;
                    }

                    byte[] decrypted = BafPbeWithMd5AndDes.Decrypt(packetData, _key);
                    IBuffer decryptedBuffer = new StreamBuffer(decrypted);
                    decryptedBuffer.SetPositionStart();
                    
                    ushort packetId = decryptedBuffer.ReadUInt16();
                    int remaining = decryptedBuffer.Size - decryptedBuffer.Position;
                    
                    byte[] payload = decryptedBuffer.ReadBytes(remaining);
                    BafPacket packet = new BafPacket(packetId, payload);
                    packets.Add(packet);
                    
                    _readPacketLength = false;
                    read = _buffer.Position != _buffer.Size;
                }
            }

            if (_buffer.Position == _buffer.Size)
            {
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