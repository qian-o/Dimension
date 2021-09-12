using DimensionService.Models.DimensionModels;
using System.Collections.Generic;

namespace DimensionService.Dao.ChatMessages
{
    public interface IChatMessagesDAO
    {
        List<ChatMessagesModel> GetChatMessages(string chatID);

        bool AddMessage(ChatMessagesModel message);

        bool MessageRead(string chatID, int messageID, string senderID);
    }
}
