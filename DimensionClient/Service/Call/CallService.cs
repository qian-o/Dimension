using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DimensionClient.Service.Call
{
    public static class CallService
    {
        public static bool CreateCall(List<string> member, ClassHelper.CallType callType, out RoomPermissionInfoModel roomPermission)
        {
            roomPermission = null;
            JObject requestObj = new()
            {
                { "Member", JArray.FromObject(member) },
                { "CallType", callType.ToString() }
            };
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/CreateCall", "POST", out JObject responseObj, requestObj: requestObj))
            {
                roomPermission = JsonConvert.DeserializeObject<RoomPermissionInfoModel>(responseObj["Data"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetUserSig(string roomID, out string userSig)
        {
            userSig = string.Empty;
            JObject requestObj = new()
            {
                { "RoomID", roomID }
            };
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/GetUserSig", "POST", out JObject responseObj, requestObj: requestObj))
            {
                userSig = responseObj["Data"].ToString();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
