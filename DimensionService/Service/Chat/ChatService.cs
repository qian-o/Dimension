using DimensionService.Common;
using DimensionService.Dao.ChatColumn;
using DimensionService.Dao.ChatLink;
using DimensionService.Dao.FriendInfo;
using DimensionService.Dao.UserInfo;
using DimensionService.Hubs;
using DimensionService.Models;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.FriendInfoModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Service.Chat
{
    public class ChatService : IChatService
    {
        private readonly IHubContext<InformHub> _hub;
        private readonly IUserInfoDAO _userInfoDAO;
        private readonly IFriendInfoDAO _friendInfoDAO;
        private readonly IChatLinkDAO _chatLinkDAO;
        private readonly IChatColumnDAO _chatColumnDAO;

        public ChatService(IHubContext<InformHub> hub, IUserInfoDAO userInfoDAO, IFriendInfoDAO friendInfoDAO, IChatLinkDAO chatLinkDAO, IChatColumnDAO chatColumnDAO)
        {
            _hub = hub;
            _userInfoDAO = userInfoDAO;
            _friendInfoDAO = friendInfoDAO;
            _chatLinkDAO = chatLinkDAO;
            _chatColumnDAO = chatColumnDAO;
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
                        _hub.Clients.Client(item.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.ChatColumnChanged.ToString(),
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
    }
}
