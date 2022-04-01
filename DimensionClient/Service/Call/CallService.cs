using Dimension.Domain;
using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace DimensionClient.Service.Call
{
    public static class CallService
    {
        public static bool CreateCall(List<string> member, CallType callType, out string roomID)
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

        public static bool GetRoomKey(string roomID, out GetRoomKeyModel roomKey)
        {
            roomKey = null;
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/GetRoomKey?RoomID={roomID}", HttpMethod.Get, out JObject responseObj))
            {
                roomKey = JsonConvert.DeserializeObject<GetRoomKeyModel>(responseObj["Data"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetRoomMember(string roomID, out List<string> member)
        {
            member = new List<string>();
            if (ClassHelper.ServerRequest($"{ClassHelper.servicePath}/api/Call/GetRoomMember?RoomID={roomID}", HttpMethod.Get, out JObject responseObj))
            {
                member = JsonConvert.DeserializeObject<List<string>>(responseObj["Data"].ToString());
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
