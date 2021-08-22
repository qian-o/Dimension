using DimensionClient.Common;
using Newtonsoft.Json.Linq;

namespace DimensionClient.Service.Chat
{
    public static class ChatService
    {
        public static bool AddChat(string friendID)
        {
            JObject requestObj = new()
            {
                { "FriendID", friendID }
            };
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/AddChat", "POST", out _, requestObj: requestObj);
        }
    }
}
