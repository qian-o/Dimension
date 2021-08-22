namespace DimensionService.Dao.ChatLink
{
    public interface IChatLinkDAO
    {
        string GetChatID(string userID1, string userID2);
    }
}
