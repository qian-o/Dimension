using DimensionService.Common;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;

namespace DimensionService.Service.Call
{
    public interface ICallService
    {
        bool CreateCall(CreateCallModel data, out string roomID, out string message);

        bool GetRoomKey(string userID, string roomID, out RoomKeyModel roomKey, out string message);

        bool GetRoomMember(string userID, string roomID, out List<string> member, out string message);

        bool NotifyRoommate(string userID, ClassHelper.UseDevice useDevice, out string message);

        bool ReplyCall(ReplyCallModel data, out string message);

        bool DissolutionRoom(string userID, ClassHelper.UseDevice useDevice, out string message);
    }
}
