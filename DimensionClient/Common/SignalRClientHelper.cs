using DimensionClient.Component.Windows;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace DimensionClient.Common
{
    public delegate void FriendOnline(string friendID, bool online);
    public delegate void NewFriend(string friendID);
    public delegate void FriendChanged(string sort, string friendID, bool state);
    public delegate void RemarkInfoChanged(string friendID);
    public delegate void ChatColumnChanged(string friendID);
    public delegate void NewMessage(string chatID);

    public static class SignalRClientHelper
    {
        private static HubConnection connection;

        #region 事件
        // 好友在线状态
        private static FriendOnline GetFriendOnline;
        public static event FriendOnline FriendOnlineSignalR
        {
            add
            {
                GetFriendOnline += value;
            }
            remove
            {
                GetFriendOnline -= value;
            }
        }
        // 新好友
        private static NewFriend GetNewFriend;
        public static event NewFriend NewFriendSignalR
        {
            add
            {
                GetNewFriend += value;
            }
            remove
            {
                GetNewFriend -= value;
            }
        }
        // 好友状态改变
        private static FriendChanged GetFriendChanged;
        public static event FriendChanged FriendChangedSignalR
        {
            add
            {
                GetFriendChanged += value;
            }
            remove
            {
                GetFriendChanged -= value;
            }
        }
        // 好友备注改变
        private static RemarkInfoChanged GetRemarkInfoChanged;
        public static event RemarkInfoChanged RemarkInfoChangedSignalR
        {
            add
            {
                GetRemarkInfoChanged += value;
            }
            remove
            {
                GetRemarkInfoChanged -= value;
            }
        }
        // 聊天列表改变
        private static ChatColumnChanged GetChatColumnChanged;
        public static event ChatColumnChanged ChatColumnChangedSignalR
        {
            add
            {
                GetChatColumnChanged += value;
            }
            remove
            {
                GetChatColumnChanged -= value;
            }
        }
        // 新消息
        private static NewMessage GetNewMessage;
        public static event NewMessage NewMessageSignalR
        {
            add
            {
                GetNewMessage += value;
            }
            remove
            {
                GetNewMessage -= value;
            }
        }
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
            GetFriendOnline?.Invoke(friendID, online);
        }

        private static void Connection_NewFriend(string friendID)
        {
            GetNewFriend?.Invoke(friendID);
        }

        private static void Connection_FriendChanged(string sort, string friendID, bool state)
        {
            GetFriendChanged?.Invoke(sort, friendID, state);
        }

        private static void Connection_OnlineStatus(bool online)
        {
            ClassHelper.commonView.OnLine = online;
        }

        private static void Connection_RemarkInfoChanged(string friendID)
        {
            GetRemarkInfoChanged?.Invoke(friendID);
        }

        private static void Connection_ChatColumnChanged(string friendID)
        {
            GetChatColumnChanged?.Invoke(friendID);
        }

        private static void Connection_NewMessage(string chatID)
        {
            GetNewMessage?.Invoke(chatID);
        }
    }
}
