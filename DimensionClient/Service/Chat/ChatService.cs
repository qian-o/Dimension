using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

        public static bool GetChatColumnInfo(out List<ChatColumnInfoModel> chatColumnInfos)
        {
            chatColumnInfos = null;
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Chat/GetChatColumnInfo", "GET", out JObject responseObj))
            {
                chatColumnInfos = JsonConvert.DeserializeObject<List<ChatColumnInfoModel>>(responseObj["Data"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
