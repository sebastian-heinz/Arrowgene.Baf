namespace Arrowgene.Baf.Server.Packet
{
    public enum PacketId : ushort
    {
        Unknown = 0,
        LoginReq = 1000,
        LoginRes = 1001,
        ChannelListReq = 1002,
        ChannelListRes = 1003,
        JoinChannelReq = 1004,
        JoinChannelRes = 1005, // 0x03ED
        InitialReq = 1011,
        InitialRes = 1012,
        ProfileReq = 1029,
        ProfileRes = 1030,
        LobbyProfileReq = 2000,
        LobbyProfileRes = 2001,
        RoomListReq = 2002,
        RoomListRes = 2003,
        CreateRoomReq = 2004,
        CreateRoomRes = 2005,
        ChannelChatReq = 2012,
        ChannelChatRes = 2013,
        JoinRoomReq = 3002,
        JoinRoomRes = 3003,
    }
}