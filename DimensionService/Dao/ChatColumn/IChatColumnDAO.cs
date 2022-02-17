using DimensionService.Models.DimensionModels;

namespace DimensionService.Dao.ChatColumn
{
    public interface IChatColumnDAO
    {
        bool AddChatColumn(string userID, string friendID, string chatID);

        List<ChatColumnModel> GetChatColumns(string userID);

        bool ChatColumnExist(string userID, string chatID);
    }
}
