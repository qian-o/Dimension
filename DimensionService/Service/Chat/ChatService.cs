using Dimension.Domain;
using DimensionService.Common;
using DimensionService.Dao.ChatColumn;
using DimensionService.Dao.ChatLink;
using DimensionService.Dao.ChatMessages;
using DimensionService.Dao.FriendInfo;
using DimensionService.Dao.UserInfo;
using DimensionService.Hubs;
using DimensionService.Models;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.FriendInfoModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using Microsoft.AspNetCore.SignalR;

namespace DimensionService.Service.Chat
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<InformHub> _hub;
        private readonly IUserInfoDAO _userInfoDAO;
        private readonly IFriendInfoDAO _friendInfoDAO;
        private readonly IChatLinkDAO _chatLinkDAO;
        private readonly IChatColumnDAO _chatColumnDAO;
        private readonly IChatMessagesDAO _chatMessagesDAO;

        public ChatService(IHubContext<InformHub> hub, IUserInfoDAO userInfoDAO, IFriendInfoDAO friendInfoDAO, IChatLinkDAO chatLinkDAO, IChatColumnDAO chatColumnDAO, IChatMessagesDAO chatMessagesDAO)
        {
            _hub = hub;
            _userInfoDAO = userInfoDAO;
            _friendInfoDAO = friendInfoDAO;
            _chatLinkDAO = chatLinkDAO;
            _chatColumnDAO = chatColumnDAO;
            _chatMessagesDAO = chatMessagesDAO;
        }

        public bool AddChat(AddChatModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (_friendInfoDAO.ConfirmFriend(data.UserID, data.FriendID))
                {
                    string chatID = _chatLinkDAO.GetChatID(data.UserID, data.FriendID);
                    _chatColumnDAO.AddChatColumn(data.UserID, data.FriendID, chatID);
                    foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(item => item.UserID == data.UserID))
                    {
                        _hub.Clients.Client(item.ConnectionId).SendAsync(method: HubMessageType.ChatColumnChanged.ToString(),
                                                                             arg1: data.FriendID);
                    }
                    state = true;
                }
                else
                {
                    message = "好友关系不存在";
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetChatColumnInfo(string userID, out List<ChatColumnInfoModel> chatColumnInfos, out string message)
        {
            try
            {
                bool state = false;
                chatColumnInfos = new();
                message = string.Empty;
                List<ChatColumnModel> chatColumns = _chatColumnDAO.GetChatColumns(userID);
                List<FriendModel> friendModels = _friendInfoDAO.GetFriends(userID);
                chatColumnInfos.AddRange(_userInfoDAO.GetUserInfos(chatColumns.Select(item => item.FriendID).ToList()).Select(item => new ChatColumnInfoModel
                {
                    FriendID = item.UserID,
                    NickName = item.NickName,
                    RemarkName = friendModels.Find(friend => friend.UserID == item.UserID).RemarkName,
                    HeadPortrait = item.HeadPortrait,
                    ChatID = chatColumns.Find(chat => chat.FriendID == item.UserID).ChatID
                }));
                state = true;
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetChattingRecords(string userID, string chatID, out List<ChatMessagesModel> chatMessages, out string message)
        {
            try
            {
                bool state = false;
                chatMessages = null;
                message = string.Empty;
                if (_chatLinkDAO.ConfirmChatID(userID, chatID))
                {
                    chatMessages = _chatMessagesDAO.GetChatMessages(chatID);
                    state = true;
                }
                else
                {
                    message = "该聊天室不属于您。";
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendMessage(SendMessageModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (_chatLinkDAO.GetPeerID(data.UserID, data.ChatID) is string receiverID)
                {
                    ChatMessagesModel chatMessages = new()
                    {
                        ChatID = data.ChatID,
                        SenderID = data.UserID,
                        ReceiverID = receiverID,
                        MessageType = data.MessageType,
                        MessageContent = data.MessageContent,
                        CreateTime = DateTime.Now
                    };
                    if (_chatMessagesDAO.AddMessage(chatMessages))
                    {
                        if (!_chatColumnDAO.ChatColumnExist(receiverID, data.ChatID))
                        {
                            _chatColumnDAO.AddChatColumn(receiverID, data.UserID, data.ChatID);
                            foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(item => item.UserID == receiverID))
                            {
                                _hub.Clients.Client(item.ConnectionId).SendAsync(method: HubMessageType.ChatColumnChanged.ToString(),
                                                                                     arg1: data.UserID);
                            }
                        }
                        foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(item => item.UserID == data.UserID || item.UserID == receiverID))
                        {
                            _hub.Clients.Client(item.ConnectionId).SendAsync(method: HubMessageType.NewMessage.ToString(),
                                                                                 arg1: data.ChatID);
                        }
                        state = true;
                    }
                    else
                    {
                        message = "消息发送失败。";
                    }
                }
                else
                {
                    message = "该聊天室不属于您。";
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ReadMessage(ReadMessageModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (_chatLinkDAO.GetPeerID(data.UserID, data.ChatID) is string senderID)
                {
                    _chatMessagesDAO.MessageRead(data.ChatID, data.MessageID, senderID);
                    state = true;
                }
                else
                {
                    message = "该聊天室不属于您。";
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
