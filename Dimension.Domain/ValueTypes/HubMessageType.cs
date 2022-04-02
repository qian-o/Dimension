using System.ComponentModel;

namespace Dimension.Domain
{
    // SignalR消息类别
    public enum HubMessageType
    {
        // string title, string message
        [Description("通知")]
        Notification,
        [Description("相同设备登录冲突")]
        DeviceLoginConflict,
        // string friendID, bool online
        [Description("好友在线状态")]
        FriendOnline,
        // string friendID
        [Description("新好友")]
        NewFriend,
        // string sort, string friendID, bool state ( true 添加 false 删除 )
        [Description("好友发生改变")]
        FriendChanged,
        // bool online
        [Description("在线状态改变")]
        OnlineStatus,
        // string friendID
        [Description("备注信息改变")]
        RemarkInfoChanged,
        // string friendID
        [Description("聊天列表改变")]
        ChatColumnChanged,
        // string chatID
        [Description("新消息")]
        NewMessage,
        // string userID, CallType callType, string roomID
        [Description("呼叫")]
        CallInvite,
        // string roomID
        [Description("任意设备处理")]
        OtherDeviceProcessed,
        // string userID, bool isAcceptCall
        [Description("是否接收应答")]
        AcceptCall
    }
}
