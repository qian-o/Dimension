using DimensionService.Models.DimensionModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;

namespace DimensionService.Service.UserManager
{
    public interface IUserManagerService
    {
        bool UserLogin(UserLoginModel data, out LoginInfoModel login, out string message);

        bool GetVerificationCode(GetVerificationCodeModel data, out string message);

        bool PhoneNumberLogin(PhoneNumberLoginModel data, out LoginInfoModel login, out string message);

        bool GetUserInfo(string userID, out UserInfoModel user, out string message);

        bool GetFriendList(string userID, out List<FriendSortModel> friendSorts, out string message);

        bool FriendRegistration(FriendRegistrationModel data, out string message);

        bool FriendValidation(FriendValidationModel data, out string message);

        bool GetNewFriendList(string userID, out List<NewFriendBriefModel> newFriendBriefs, out string message);

        bool GetFriendInfo(GetFriendInfoModel data, out FriendDetailsModel friendDetails, out string message);

        bool UpdateRemarkInfo(UpdateRemarkInfoModel data, out string message);
    }
}
