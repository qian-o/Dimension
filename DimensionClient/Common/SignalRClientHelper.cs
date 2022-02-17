using DimensionClient.Component.Windows;
using DimensionClient.Service.UserManager;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace DimensionClient.Common
{
    public delegate void FriendOnline(string friendID, bool online);
    public delegate void NewFriend(string friendID);
    public delegate void FriendChanged(string sort, string friendID, bool state);
    public delegate void RemarkInfoChanged(string friendID);
    public delegate void ChatColumnChanged(string friendID);
    public delegate void NewMessage(string chatID);
    public delegate void CallInvite(string userID, ClassHelper.CallType callType, string roomID);
    public delegate void OtherDeviceProcessed(string roomID);
    public delegate void AcceptCall(string userID, bool isAcceptCall);

    public static class SignalRClientHelper
    {
        private static HubConnection connection;

        #region 事件
        // 好友在线状态
        public static event FriendOnline FriendOnlineSignalR;
        // 新好友
        public static event NewFriend NewFriendSignalR;
        // 好友状态改变
        public static event FriendChanged FriendChangedSignalR;
        // 好友备注改变
        public static event RemarkInfoChanged RemarkInfoChangedSignalR;
        // 聊天列表改变
        public static event ChatColumnChanged ChatColumnChangedSignalR;
        // 新消息
        public static event NewMessage NewMessageSignalR;
        // 通话邀请
        public static event CallInvite CallInviteSignalR;
        // 其他设备处理
        public static event OtherDeviceProcessed OtherDeviceProcessedSignalR;
        // 是否应答通话
        public static event AcceptCall AcceptCallSignalR;
        #endregion

        public static async void InitializeConnection()
        {
            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl($"{ClassHelper.servicePath}/InformHub?UserID={ClassHelper.UserID}&Token={ClassHelper.Token}&Device={ClassHelper.device}", HttpTransportType.WebSockets, (httpConnectionOptions) => { httpConnectionOptions.SkipNegotiation = true; })
                    .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
                    .Build();
                connection.Closed += Connection_Closed;
                connection.On<string, string>(ClassHelper.HubMessageType.Notification.ToString(), Connection_Notification);
                connection.On<string, bool>(ClassHelper.HubMessageType.FriendOnline.ToString(), Connection_FriendOnline);
                connection.On<string>(ClassHelper.HubMessageType.NewFriend.ToString(), Connection_NewFriend);
                connection.On<string, string, bool>(ClassHelper.HubMessageType.FriendChanged.ToString(), Connection_FriendChanged);
                connection.On<bool>(ClassHelper.HubMessageType.OnlineStatus.ToString(), Connection_OnlineStatus);
                connection.On<string>(ClassHelper.HubMessageType.RemarkInfoChanged.ToString(), Connection_RemarkInfoChanged);
                connection.On<string>(ClassHelper.HubMessageType.ChatColumnChanged.ToString(), Connection_ChatColumnChanged);
                connection.On<string>(ClassHelper.HubMessageType.NewMessage.ToString(), Connection_NewMessage);
                connection.On<string, ClassHelper.CallType, string>(ClassHelper.HubMessageType.CallInvite.ToString(), Connection_CallInvite);
                connection.On<string>(ClassHelper.HubMessageType.OtherDeviceProcessed.ToString(), Connection_OtherDeviceProcessed);
                connection.On<string, bool>(ClassHelper.HubMessageType.AcceptCall.ToString(), Connection_AcceptCall);
                await connection.StartAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Task Connection_Closed(Exception arg)
        {
            ClassHelper.RecordException(typeof(SignalRClientHelper), arg);
            return Task.CompletedTask;
        }

        private static void Connection_Notification(string title, string message)
        {
            ClassHelper.NotificationAlert(typeof(MainWindow), title, message);
        }

        private static void Connection_FriendOnline(string friendID, bool online)
        {
            FriendOnlineSignalR?.Invoke(friendID, online);
        }

        private static void Connection_NewFriend(string friendID)
        {
            NewFriendSignalR?.Invoke(friendID);
        }

        private static void Connection_FriendChanged(string sort, string friendID, bool state)
        {
            FriendChangedSignalR?.Invoke(sort, friendID, state);
        }

        private static void Connection_OnlineStatus(bool online)
        {
            ClassHelper.commonView.OnLine = online;
        }

        private static void Connection_RemarkInfoChanged(string friendID)
        {
            RemarkInfoChangedSignalR?.Invoke(friendID);
        }

        private static void Connection_ChatColumnChanged(string friendID)
        {
            ChatColumnChangedSignalR?.Invoke(friendID);
        }

        private static void Connection_NewMessage(string chatID)
        {
            NewMessageSignalR?.Invoke(chatID);
        }

        private static void Connection_CallInvite(string userID, ClassHelper.CallType callType, string roomID)
        {
            if (UserManagerService.FriendValidation(userID, true))
            {
                CallInviteSignalR?.Invoke(userID, callType, roomID);
            }
        }

        private static void Connection_OtherDeviceProcessed(string roomID)
        {
            OtherDeviceProcessedSignalR?.Invoke(roomID);
        }

        private static void Connection_AcceptCall(string userID, bool isAcceptCall)
        {
            AcceptCallSignalR?.Invoke(userID, isAcceptCall);
        }
    }
}
