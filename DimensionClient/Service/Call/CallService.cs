using DimensionClient.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DimensionClient.Service.Call
{
    public static class CallService
    {
        public static bool CreateCall(List<string> member, ClassHelper.CallType callType, out string roomID)
        {
            roomID = string.Empty;
            JObject requestObj = new()
            {
                { "Member", JArray.FromObject(member) },
                { "CallType", callType.ToString() }
            };
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/CreateCall", "POST", out JObject responseObj, requestObj: requestObj))
            {
                roomID = responseObj["Data"].ToString();
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
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/GetUserSig?RoomID={roomID}", "GET", out JObject responseObj))
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
