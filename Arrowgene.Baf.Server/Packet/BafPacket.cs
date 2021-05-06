using System;
using System.Text;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Buffers;

namespace Arrowgene.Baf.Server.Packet
{
    public class BafPacket
    {
        public byte[] Data;
        public PacketId Id { get; }
        public ushort IdValue { get; }
        public PacketSource Source { get; }

        public BafPacket(PacketId id, byte[] data, PacketSource source = PacketSource.Server)
        {
            Id = id;
            Data = data;
            Source = source;
            IdValue = (ushort) id;
        }

        public BafPacket(ushort id, byte[] data, PacketSource source = PacketSource.Server)
        {
            IdValue = id;
            Data = data;
            Source = source;
            if (Enum.IsDefined(typeof(PacketId), id))
            {
                Id = (PacketId) id;
            }
            else
            {
                Id = PacketId.Unknown;
            }
        }

        public IBuffer CreateBuffer()
        {
            return new StreamBuffer(Data);
        }

        public string AsString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"==={Environment.NewLine}");
            sb.Append($"Id:[{Id} {IdValue} {IdValue:X}]{Environment.NewLine}");
            sb.Append($"Source:[{Source}]{Environment.NewLine}");
            sb.Append($"Data:{Environment.NewLine}{Util.HexDump(Data)}");
            sb.Append("===");
            return sb.ToString();
        }
    }
}