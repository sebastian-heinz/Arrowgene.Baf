namespace Arrowgene.Baf.Server.Packet
{
    public enum PacketId : ushort
    {
        Unknown = 0,
        LoginReq = 1000,
        LoginRes = 1001,
        ChannelListReq = 1002,
        ChannelListRes = 1003,
        InitialReq = 1011,
        InitialRes = 1012,
    }
}