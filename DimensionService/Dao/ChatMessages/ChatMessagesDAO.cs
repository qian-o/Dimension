using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using Microsoft.EntityFrameworkCore;

namespace DimensionService.Dao.ChatMessages
{
    public class ChatMessagesDAO : IChatMessagesDAO
    {
        public List<ChatMessagesModel> GetChatMessages(string chatID)
        {
            using DimensionContext context = new();
            return context.ChatMessages.Where(item => item.ChatID == chatID && item.IsVisible != -1 && !item.IsWithdraw).ToList();
        }

        public bool AddMessage(ChatMessagesModel message)
        {
            using DimensionContext context = new();
            context.ChatMessages.Add(message);
            return context.SaveChanges() > 0;
        }

        public bool MessageRead(string chatID, int messageID, string senderID)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    foreach (ChatMessagesModel item in context.ChatMessages.Where(c => c.ChatID == chatID && c.ID <= messageID && c.SenderID == senderID && !c.IsRead && c.IsVisible != -1 && !c.IsWithdraw))
                    {
                        item.IsRead = true;
                    }
                    context.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
            return saved;
        }
    }
}
