using DimensionService.Models.DimensionModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using System.Collections.Generic;

namespace DimensionService.Service.Chat
{
    public interface IChatService
    {
        bool AddChat(AddChatModel data, out string message);

        bool GetChatColumnInfo(string userID, out List<ChatColumnInfoModel> chatColumnInfos, out string message);

        bool GetChattingRecords(string userID, string chatID, out List<ChatMessagesModel> chatMessages, out string message);

        bool SendMessage(SendMessageModel data, out string message);

        bool ReadMessage(ReadMessageModel data, out string message);
    }
}
