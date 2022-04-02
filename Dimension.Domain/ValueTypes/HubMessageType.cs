namespace Dimension.Domain
{
    // SignalR消息类别
    public enum HubMessageType
    {
        // string title, string message
        Notification,
        // string friendID, bool online
        FriendOnline,
        // string friendID
        NewFriend,
        // string sort, string friendID, bool state ( true 添加 false 删除 )
        FriendChanged,
        // bool online
        OnlineStatus,
        // string friendID
        RemarkInfoChanged,
        // string friendID
        ChatColumnChanged,
        // string chatID
        NewMessage,
        // string userID, CallType callType, string roomID
        CallInvite,
        // string roomID
        OtherDeviceProcessed,
        // string userID, bool isAcceptCall
        AcceptCall
    }
}
