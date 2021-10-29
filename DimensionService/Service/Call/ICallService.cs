using DimensionService.Common;
using DimensionService.Models.RequestModels;

namespace DimensionService.Service.Call
{
    public interface ICallService
    {
        bool CreateCall(CreateCallModel data, out string roomID, out string message);

        bool GetUserSig(string userID, string roomID, out string userSig, out string message);

        bool NotifyRoommate(string userID, ClassHelper.UseDevice useDevice, out string message);

        bool ReplyCall(ReplyCallModel data, out string message);

        bool DissolutionRoom(string userID, ClassHelper.UseDevice useDevice, out string message);
    }
}
