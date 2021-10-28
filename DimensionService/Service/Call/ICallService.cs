using DimensionService.Models.RequestModels;

namespace DimensionService.Service.Call
{
    public interface ICallService
    {
        bool CreateCall(CreateCallModel data, out string roomID, out string message);
        bool GetUserSig(string userID, string roomID, out string userSig, out string message);
    }
}
