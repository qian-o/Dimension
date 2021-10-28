using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.CallRoomModels;
using DimensionService.Models.DimensionModels.FriendInfoModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Dao.UserInfo
{
    public class UserInfoDAO : IUserInfoDAO
    {
        public UserInfoModel UserInfoFindForUserID(string userID, bool verify = false)
        {
            using DimensionContext context = new();
            UserInfoModel userInfoModel = context.UserInfo.Where(item => item.UserID == userID).FirstOrDefault();
            if (userInfoModel != null && !verify)
            {
                userInfoModel.Password = string.Empty;
            }
            return userInfoModel;
        }

        public UserInfoModel UserInfoFindForNickName(string nickName, bool verify = false)
        {
            using DimensionContext context = new();
            UserInfoModel userInfoModel = context.UserInfo.Where(item => item.NickName == nickName).FirstOrDefault();
            if (userInfoModel != null && !verify)
            {
                userInfoModel.Password = string.Empty;
            }
            return userInfoModel;
        }

        public UserInfoModel UserInfoFindForPhoneNumber(string phoneNumber, bool verify = false)
        {
            using DimensionContext context = new();
            UserInfoModel userInfoModel = context.UserInfo.Where(item => item.PhoneNumber == phoneNumber).FirstOrDefault();
            if (userInfoModel != null && !verify)
            {
                userInfoModel.Password = string.Empty;
            }
            return userInfoModel;
        }

        public UserInfoModel UserInfoFindForEmail(string email, bool verify = false)
        {
            using DimensionContext context = new();
            UserInfoModel userInfoModel = context.UserInfo.Where(item => item.Email == email).FirstOrDefault();
            if (userInfoModel != null && !verify)
            {
                userInfoModel.Password = string.Empty;
            }
            return userInfoModel;
        }

        public bool PhoneNumberExist(string phoneNumber)
        {
            using DimensionContext context = new();
            return context.UserInfo.Where(item => item.PhoneNumber == phoneNumber).Any();
        }

        public bool EmailExist(string email)
        {
            using DimensionContext context = new();
            return context.UserInfo.Where(item => item.Email == email).Any();
        }

        public bool UserInfoAdd(UserInfoModel userInfo)
        {
            using DimensionContext context = new();
            context.UserInfo.Add(userInfo);
            context.FriendInfo.Add(new FriendInfoModel
            {
                UserID = userInfo.UserID,
                Friends = JArray.FromObject(new List<FriendModel>()).ToString(),
                NewFriends = JArray.FromObject(new List<NewFriendModel>()).ToString()
            });
            context.CallRoom.Add(new CallRoomModel
            {
                HouseOwnerID = userInfo.UserID,
                RoomID = $"{userInfo.UserID}_Room",
                Roommate = JArray.FromObject(new List<RoommateModel>()).ToString(),
                Enabled = false
            });
            return context.SaveChanges() > 0;
        }

        public bool UpdatedOnLine(string userID, bool onLine)
        {
            using DimensionContext context = new();
            if (context.UserInfo.Where(item => item.UserID == userID).FirstOrDefault() is UserInfoModel userInfo)
            {
                userInfo.OnLine = onLine;
            }
            return context.SaveChanges() > 0;
        }

        public List<UserInfoModel> GetUserInfos(List<string> userIDs)
        {
            using DimensionContext context = new();
            List<UserInfoModel> userInfos = context.UserInfo.Where(item => userIDs.Any(u => u == item.UserID)).ToList();
            foreach (UserInfoModel item in userInfos)
            {
                item.Password = string.Empty;
            }
            return userInfos;
        }
    }
}
