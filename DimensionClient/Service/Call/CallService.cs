using DimensionClient.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;

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
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/CreateCall", HttpMethod.Post, out JObject responseObj, requestObj: requestObj))
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
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/GetUserSig?RoomID={roomID}", HttpMethod.Get, out JObject responseObj))
            {
                userSig = responseObj["Data"].ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool NotifyRoommate()
        {
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/NotifyRoommate", HttpMethod.Post, out JObject _);
        }

        public static bool ReplyCall(string roomID, bool isAcceptCall)
        {
            JObject requestObj = new()
            {
                { "RoomID", roomID },
                { "IsAcceptCall", isAcceptCall }
            };
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/ReplyCall", HttpMethod.Post, out JObject _, requestObj: requestObj);
        }

        public static bool DissolutionRoom()
        {
            return ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/DissolutionRoom", HttpMethod.Post, out JObject _);
        }
    }
}
