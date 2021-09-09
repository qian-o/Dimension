using DimensionService.Common;
using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Dao.ChatLink
{
    public class ChatLinkDAO : IChatLinkDAO
    {
        private static readonly Dictionary<string, object> getChatIDLocks = new();
        private static readonly object getChatIDLock = new();

        public string GetChatID(string userID1, string userID2)
        {
            object bodyLock;
            lock (getChatIDLock)
            {
                string str = $"{userID1}-{userID2}";
                if (!getChatIDLocks.TryGetValue($"{userID1}-{userID2}", out bodyLock))
                {
                    if (!getChatIDLocks.TryGetValue($"{userID2}-{userID1}", out bodyLock))
                    {
                        bodyLock = new object();
                        getChatIDLocks.TryAdd(str, bodyLock);
                    }
                }
            }
            lock (bodyLock)
            {
                using DimensionContext context = new();
                if (context.ChatLink.Where(item => item.UserID1 == userID1 && item.UserID2 == userID2).FirstOrDefault() is ChatLinkModel chatLink1)
                {
                    return chatLink1.ChatID;
                }
                else if (context.ChatLink.Where(item => item.UserID1 == userID2 && item.UserID2 == userID1).FirstOrDefault() is ChatLinkModel chatLink2)
                {
                    return chatLink2.ChatID;
                }
                else
                {
                    context.ChatLink.Add(new ChatLinkModel
                    {
                        UserID1 = userID1,
                        UserID2 = userID2,
                        ChatID = $"{ClassHelper.GetRandomString(10)}-{ClassHelper.GetRandomString(10)}-{ClassHelper.GetRandomString(10)}"
                    });
                    context.SaveChanges();
                    return context.ChatLink.Where(item => item.UserID1 == userID1 && item.UserID2 == userID2).FirstOrDefault().ChatID;
                }
            }
        }

        public bool ConfirmChatID(string userID, string chatID)
        {
            using DimensionContext context = new();
            return context.ChatLink.FirstOrDefault(item => item.ChatID == chatID) is ChatLinkModel chatLink && (chatLink.UserID1 == userID || chatLink.UserID2 == userID);
        }
    }
}
