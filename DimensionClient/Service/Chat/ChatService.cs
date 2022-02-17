using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

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
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/AddChat", HttpMethod.Post, out _, requestObj: requestObj);
        }

        public static bool GetChatColumnInfo(out List<ChatColumnInfoModel> chatColumnInfos)
        {
            chatColumnInfos = null;
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/GetChatColumnInfo", HttpMethod.Get, out JObject responseObj))
            {
                chatColumnInfos = JsonConvert.DeserializeObject<List<ChatColumnInfoModel>>(responseObj["Data"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetChattingRecords(string chatID, out List<ChatMessagesModel> chatMessages)
        {
            chatMessages = null;
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/GetChattingRecords?chatID={chatID}", HttpMethod.Get, out JObject responseObj))
            {
                chatMessages = JsonConvert.DeserializeObject<List<ChatMessagesModel>>(responseObj["Data"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SendMessage(string chatID, ClassHelper.MessageType messageType, string messageContent)
        {
            JObject requestObj = new()
            {
                { "ChatID", chatID },
                { "MessageType", messageType.ToString() },
                { "MessageContent", messageContent }
            };
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/SendMessage", HttpMethod.Post, out _, requestObj: requestObj);
        }

        public static bool ReadMessage(string chatID, int messageID)
        {
            JObject requestObj = new()
            {
                { "ChatID", chatID },
                { "MessageID", messageID }
            };
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/ReadMessage", HttpMethod.Post, out _, requestObj: requestObj);
        }
    }
}
