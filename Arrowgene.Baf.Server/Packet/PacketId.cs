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
        Unknown0Req = 11000,
        Unknown0Res = 11001,
        
        
        Unknown1Req = 11005,
        Unknown1Res = 11006,
        
        
        Unknown2Req = 11010,
        Unknown2Res = 11011,
        
        
        
        ProfileReq = 1029,
        ProfileRes = 1030,
        MailSendReq = 1041,
        MailSendRes = 1042,
        CreateBandReq = 1063,
        CreateBandRes = 1064,
        
        // lobby
        LobbyProfileReq = 2000,
        LobbyProfileRes = 2001,
        RoomListReq = 2002,
        RoomListRes = 2003,
        CreateRoomReq = 2004,
        AnnounceRoomRes = 2005,
        CreateRoomRes = 2006,
        ChannelChatReq = 2012,
        ChannelChatRes = 2013,
        
        // room
        RoomChangeSettingReq = 3000,
        RoomChangeSettingRes = 3001,
        RoomJoinReq = 3002,
        RoomJoinRes = 3003,
        RoomLeaveReq = 3005,
        RoomLeaveRes = 3006,
        RoomModifySlotReq = 3008,
        RoomModifySlotRes = 3009,
        RoomChatReq = 3011,
        RoomChatRes = 3012,
        RoomSelectSongReq = 3016,
        RoomSelectSongRes = 3017,
        RoomStartSongReq = 3032,
        RoomStartSongRes = 3033,
        RoomChangeColorReq = 3028,
        RoomChangeColorRes = 3029,
        RoomSpectatorReq = 3053,
        RoomSpectatorRes = 3054,
        RoomSelectModeReq = 3075,
        RoomSelectModeRes = 3076,
        
        // shop
        ShopBuyItemReq = 8002,
        ShopBuyItemRes = 8003
    }
}