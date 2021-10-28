using DimensionService.Common;
using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Dao.CallRoom
{
    public class CallRoomDAO : ICallRoomDAO
    {
        public bool GetRoomStatus(string houseOwnerID)
        {
            using DimensionContext context = new();
            return context.CallRoom.FirstOrDefault(item => item.HouseOwnerID == houseOwnerID).Enabled;
        }

        public CallRoomModel GetCallRoomForHouseOwnerID(string houseOwnerID)
        {
            using DimensionContext context = new();
            return context.CallRoom.FirstOrDefault(item => item.HouseOwnerID == houseOwnerID);
        }

        public CallRoomModel GetCallRoomForRoomID(string roomID)
        {
            using DimensionContext context = new();
            return context.CallRoom.FirstOrDefault(item => item.RoomID == roomID);
        }

        public bool UpdatedCallRoom(string houseOwnerID, ClassHelper.CallType callType, List<string> member, bool enabled)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    if (context.CallRoom.FirstOrDefault(item => item.HouseOwnerID == houseOwnerID) is CallRoomModel callRoom)
                    {
                        callRoom.CallType = callType;
                        List<RoommateModel> roommates = new();
                        roommates.AddRange(member.Select(item => new RoommateModel
                        {
                            UserID = item,
                            UserSig = ClassHelper.GetCallAuthorization(item, callRoom.RoomID, callType),
                            EnterRoom = false
                        }));
                        callRoom.Roommate = JArray.FromObject(roommates).ToString();
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
    }
}
