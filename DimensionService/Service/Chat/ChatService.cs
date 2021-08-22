using DimensionService.Dao.ChatColumn;
using DimensionService.Dao.ChatLink;
using DimensionService.Dao.FriendInfo;
using DimensionService.Models.RequestModels;
using System;

namespace DimensionService.Service.Chat
{
    public class ChatService : IChatService
    {
        private readonly IFriendInfoDAO _friendListDAO;
        private readonly IChatLinkDAO _chatLinkDAO;
        private readonly IChatColumnDAO _chatColumnDAO;

        public ChatService(IFriendInfoDAO friendListDAO, IChatLinkDAO chatLinkDAO, IChatColumnDAO chatColumnDAO)
        {
            _friendListDAO = friendListDAO;
            _chatLinkDAO = chatLinkDAO;
            _chatColumnDAO = chatColumnDAO;
        }

        public bool AddChat(AddChatModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (_friendListDAO.ConfirmFriend(data.UserID, data.FriendID))
                {
                    string chatID = _chatLinkDAO.GetChatID(data.UserID, data.FriendID);
                    _chatColumnDAO.AddChatColumn(data.UserID, data.FriendID, chatID);
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
    }
}
