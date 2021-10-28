using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;

namespace DimensionService.Service.Call
{
    public interface ICallService
    {
        bool CreateCall(CreateCallModel data, out RoomPermissionInfoModel roomPermission, out string message);
        bool GetUserSig(GetUserSigModel data, out string userSig, out string message);
    }
}
