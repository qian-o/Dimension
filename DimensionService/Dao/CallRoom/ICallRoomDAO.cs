using DimensionService.Common;
using DimensionService.Models.DimensionModels;
using System.Collections.Generic;

namespace DimensionService.Dao.CallRoom
{
    public interface ICallRoomDAO
    {
        CallRoomModel GetCallRoomForHouseOwner(string houseOwnerID, ClassHelper.UseDevice houseOwnerDevice);

        CallRoomModel GetCallRoomForRoomID(string roomID);

        bool UpdateCallRoom(string houseOwnerID, ClassHelper.UseDevice houseOwnerDevice, ClassHelper.CallType? callType, List<string> member, bool enabled);

        bool UpdateRoommateStatus(string userID, string roomID, bool state, out string message);
    }
}
