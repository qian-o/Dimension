using DimensionService.Common;
using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimensionService.Dao.CallRoom
{
    public class CallRoomDAO : ICallRoomDAO
    {
        public CallRoomModel GetCallRoomForHouseOwner(string houseOwnerID, ClassHelper.UseDevice houseOwnerDevice)
        {
            using DimensionContext context = new();
            return context.CallRoom.FirstOrDefault(item => item.HouseOwnerID == houseOwnerID && item.HouseOwnerDevice == houseOwnerDevice);
        }

        public CallRoomModel GetCallRoomForRoomID(string roomID)
        {
            using DimensionContext context = new();
            return context.CallRoom.FirstOrDefault(item => item.RoomID == roomID);
        }

        public bool UpdateCallRoom(string houseOwnerID, ClassHelper.UseDevice houseOwnerDevice, ClassHelper.CallType? callType, List<string> member, bool enabled)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    if (context.CallRoom.FirstOrDefault(item => item.HouseOwnerID == houseOwnerID && item.HouseOwnerDevice == houseOwnerDevice) is CallRoomModel callRoom)
                    {
                        callRoom.HouseCallType = callType;
                        if (enabled)
                        {
                            List<RoommateModel> roommates = new();
                            roommates.AddRange(member.Select(item => new RoommateModel
                            {
                                UserID = item,
                                UserSig = ClassHelper.GetCallAuthorization(item, callRoom.RoomID, callType.Value, out string privateMapKey, createRoom: item == callRoom.HouseOwnerID),
                                PrivateMapKey = privateMapKey,
                                IsEnter = item == callRoom.HouseOwnerID ? true : null
                            }));
                            callRoom.Roommate = JArray.FromObject(roommates).ToString();
                        }
                        else
                        {
                            callRoom.Roommate = JArray.FromObject(new List<RoommateModel>()).ToString();
                        }
                        callRoom.Enabled = enabled;
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

        public bool UpdateRoommateStatus(string userID, string roomID, bool state, out string message)
        {
            bool saved = false;
            message = string.Empty;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    if (context.CallRoom.FirstOrDefault(item => item.RoomID == roomID) is CallRoomModel callRoom)
                    {
                        if (callRoom.Enabled)
                        {
                            List<RoommateModel> roommates = JsonConvert.DeserializeObject<List<RoommateModel>>(callRoom.Roommate);
                            if (roommates.FirstOrDefault(item => item.UserID == userID) is RoommateModel roommate)
                            {
                                if (roommate.IsEnter == null)
                                {
                                    roommate.IsEnter = state;
                                }
                                else
                                {
                                    message = "其他终端已处理。";
                                    return false;
                                }
                            }
                            else
                            {
                                message = "您不属于当前房间。";
                                return false;
                            }
                        }
                        else
                        {
                            message = "房间已解散。";
                            return false;
                        }
                    }
                    context.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException)
                {

                    throw;
                }
            }
            return saved;
        }
    }
}
