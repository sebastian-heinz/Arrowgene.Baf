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
        private byte[] _key;
        private byte[] _iv;

        public PacketFactory(byte[] key, byte[] iv)
        {
            _key = key;
            _iv = iv;
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
                    packetData = BafPbeWithMd5AndDes.Decrypt(packetData, _key, _iv);
                    IBuffer buffer = new StreamBuffer(packetData);
                    buffer.SetPositionStart();
                    BafPacket packet = new BafPacket(buffer);
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