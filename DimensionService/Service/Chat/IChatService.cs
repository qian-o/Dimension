using DimensionService.Models.RequestModels;

namespace DimensionService.Service.Chat
{
    public interface IChatService
    {
        bool AddChat(AddChatModel data, out string message);
    }
}
