using DimensionService.Common;
using DimensionService.Context;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.FriendInfoModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DimensionService.Dao.FriendInfo
{
    public class FriendInfoDAO : IFriendInfoDAO
    {
        public bool ConfirmFriend(string userID, string friendID)
        {
            using DimensionContext context = new();
            FriendInfoModel userInfo = context.FriendInfo.Where(item => item.UserID == userID).FirstOrDefault();
            return JsonConvert.DeserializeObject<List<FriendModel>>(userInfo.Friends).Exists(friend => friend.UserID == friendID);
        }

        public List<FriendModel> GetFriends(string userID)
        {
            using DimensionContext context = new();
            FriendInfoModel userInfo = context.FriendInfo.Where(item => item.UserID == userID).FirstOrDefault();
            return JsonConvert.DeserializeObject<List<FriendModel>>(userInfo.Friends);
        }

        public bool AddFriend(string userID, string friendID, string verifyInfo)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    FriendInfoModel userInfo = context.FriendInfo.Where(item => item.UserID == userID).FirstOrDefault();
                    FriendInfoModel friendInfo = context.FriendInfo.Where(item => item.UserID == friendID).FirstOrDefault();
                    List<NewFriendModel> addFriends = JsonConvert.DeserializeObject<List<NewFriendModel>>(userInfo.NewFriends);
                    List<NewFriendModel> verifyFriends = JsonConvert.DeserializeObject<List<NewFriendModel>>(friendInfo.NewFriends);
                    if (addFriends.Find(friend => friend.FriendType == ClassHelper.NewFriendType.Verify && friend.UserID == friendID && friend.Passed == null) != null)
                    {
                        // 如果对方申请添加自己，自己再添加对方的话，直接确认好友关系
                        saved = VerifyFriend(userID, friendID, true);
                    }
                    else
                    {
                        #region 用户数据
                        if (addFriends.Find(friend => friend.FriendType == ClassHelper.NewFriendType.Add && friend.UserID == friendID && friend.Passed == null) is NewFriendModel userInfoRequest)
                        {
                            userInfoRequest.VerifyInfo = verifyInfo;
                        }
                        else
                        {
                            addFriends.Add(new NewFriendModel
                            {
                                FriendType = ClassHelper.NewFriendType.Add,
                                UserID = friendID,
                                VerifyInfo = verifyInfo
                            });
                        }
                        userInfo.NewFriends = JArray.FromObject(addFriends).ToString();
                        #endregion
                        #region 好友数据
                        if (verifyFriends.Find(friend => friend.FriendType == ClassHelper.NewFriendType.Verify && friend.UserID == userID && friend.Passed == null) is NewFriendModel friendInfoRequest)
                        {
                            friendInfoRequest.VerifyInfo = verifyInfo;
                        }
                        else
                        {
                            verifyFriends.Add(new NewFriendModel
                            {
                                FriendType = ClassHelper.NewFriendType.Verify,
                                UserID = userID,
                                VerifyInfo = verifyInfo
                            });
                        }
                        friendInfo.NewFriends = JArray.FromObject(verifyFriends).ToString();
                        #endregion
                        context.SaveChanges();
                        saved = true;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
            return saved;
        }

        public bool VerifyFriend(string userID, string friendID, bool passed)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    FriendInfoModel userInfo = context.FriendInfo.Where(item => item.UserID == userID).FirstOrDefault();
                    FriendInfoModel friendInfo = context.FriendInfo.Where(item => item.UserID == friendID).FirstOrDefault();
                    List<NewFriendModel> verifyFriends = JsonConvert.DeserializeObject<List<NewFriendModel>>(userInfo.NewFriends);
                    List<NewFriendModel> addFriends = JsonConvert.DeserializeObject<List<NewFriendModel>>(friendInfo.NewFriends);
                    if (verifyFriends.Find(item => item.FriendType == ClassHelper.NewFriendType.Verify && item.UserID == friendID && item.Passed == null) is NewFriendModel userInfoRequest && addFriends.Find(item => item.FriendType == ClassHelper.NewFriendType.Add && item.UserID == userID && item.Passed == null) is NewFriendModel friendInfoRequest)
                    {
                        userInfoRequest.Passed = passed;
                        friendInfoRequest.Passed = passed;
                        if (passed)
                        {
                            List<FriendModel> userFriends = JsonConvert.DeserializeObject<List<FriendModel>>(userInfo.Friends);
                            List<FriendModel> friendFriends = JsonConvert.DeserializeObject<List<FriendModel>>(friendInfo.Friends);
                            if (userFriends.Find(item => item.UserID == friendID) == null)
                            {
                                userFriends.Add(new FriendModel
                                {
                                    UserID = friendID,
                                    RemarkName = string.Empty,
                                    RemarkInformation = string.Empty,
                                    FoundTime = DateTime.Now
                                });
                                userInfo.Friends = JArray.FromObject(userFriends).ToString();
                            }
                            if (friendFriends.Find(item => item.UserID == userID) == null)
                            {
                                friendFriends.Add(new FriendModel
                                {
                                    UserID = userID,
                                    RemarkName = string.Empty,
                                    RemarkInformation = string.Empty,
                                    FoundTime = DateTime.Now
                                });
                                friendInfo.Friends = JArray.FromObject(friendFriends).ToString();
                            }
                        }
                        userInfo.NewFriends = JArray.FromObject(verifyFriends).ToString();
                        friendInfo.NewFriends = JArray.FromObject(addFriends).ToString();
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

        public List<NewFriendModel> GetNewFriends(string userID)
        {
            using DimensionContext context = new();
            FriendInfoModel userInfo = context.FriendInfo.Where(item => item.UserID == userID).FirstOrDefault();
            return JsonConvert.DeserializeObject<List<NewFriendModel>>(userInfo.NewFriends);
        }

        public bool UpdateRemark(string userID, string friendID, string remarkName, string remarkInformation)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using DimensionContext context = new();
                    FriendInfoModel userInfo = context.FriendInfo.Where(item => item.UserID == userID).FirstOrDefault();
                    List<FriendModel> friends = JsonConvert.DeserializeObject<List<FriendModel>>(userInfo.Friends);
                    if (friends.Find(item => item.UserID == friendID) is FriendModel friend)
                    {
                        if (remarkName != null)
                        {
                            friend.RemarkName = remarkName;
                        }
                        if (remarkInformation != null)
                        {
                            friend.RemarkInformation = remarkInformation;
                        }
                        userInfo.Friends = JArray.FromObject(friends).ToString();
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
