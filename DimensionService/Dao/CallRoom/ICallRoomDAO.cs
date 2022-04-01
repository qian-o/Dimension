using Dimension.Domain;
using DimensionService.Common;
using DimensionService.Models.DimensionModels;

namespace DimensionService.Dao.CallRoom
{
    public interface ICallRoomDAO
    {
        CallRoomModel GetCallRoomForHouseOwner(string houseOwnerID, UseDevice houseOwnerDevice);

        CallRoomModel GetCallRoomForRoomID(string roomID);

        bool UpdateCallRoom(string houseOwnerID, UseDevice houseOwnerDevice, CallType? callType, List<string> member, bool enabled);

        bool UpdateRoommateStatus(string userID, string roomID, bool state, out string message);
    }
}
