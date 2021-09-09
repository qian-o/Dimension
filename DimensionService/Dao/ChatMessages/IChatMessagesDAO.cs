using DimensionService.Models.DimensionModels;
using System.Collections.Generic;

namespace DimensionService.Dao.ChatMessages
{
    public interface IChatMessagesDAO
    {
        List<ChatMessagesModel> GetChatMessages(string chatID);
    }
}
