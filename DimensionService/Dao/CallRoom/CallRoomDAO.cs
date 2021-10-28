using DimensionService.Common;
using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

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

        public bool UpdatedCallRoom(string houseOwnerID, ClassHelper.UseDevice houseOwnerDevice, ClassHelper.CallType callType, List<string> member, bool enabled)
        {
            using DimensionContext context = new();
            if (context.CallRoom.FirstOrDefault(item => item.HouseOwnerID == houseOwnerID && item.HouseOwnerDevice == houseOwnerDevice) is CallRoomModel callRoom)
            {
                List<RoommateModel> roommates = new();
                roommates.AddRange(member.Select(item => new RoommateModel
                {
                    UserID = item,
                    UserSig = ClassHelper.GetCallAuthorization(item, callRoom.RoomID, callType, createRoom: item == callRoom.HouseOwnerID)
                }));
                callRoom.Roommate = JArray.FromObject(roommates).ToString();
                callRoom.Enabled = enabled;
            }
            return context.SaveChanges() > 0;
        }
    }
}
