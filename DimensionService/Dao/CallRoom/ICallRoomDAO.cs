using DimensionService.Common;
using DimensionService.Models.DimensionModels;
using System.Collections.Generic;

namespace DimensionService.Dao.CallRoom
{
    public interface ICallRoomDAO
    {
        bool GetRoomStatus(string houseOwnerID);

        CallRoomModel GetCallRoomForHouseOwnerID(string houseOwnerID);

        CallRoomModel GetCallRoomForRoomID(string roomID);

        bool UpdatedCallRoom(string houseOwnerID, ClassHelper.CallType callType, List<string> member, bool enabled);
    }
}
