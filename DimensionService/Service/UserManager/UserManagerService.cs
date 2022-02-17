using DimensionService.Common;
using DimensionService.Dao.FriendInfo;
using DimensionService.Dao.LoginInfo;
using DimensionService.Dao.UserInfo;
using DimensionService.Hubs;
using DimensionService.Models;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.DimensionModels.FriendInfoModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace DimensionService.Service.UserManager
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IHubContext<InformHub> _hub;
        private readonly IUserInfoDAO _userInfoDAO;
        private readonly ILoginInfoDAO _loginInfoDAO;
        private readonly IFriendInfoDAO _friendInfoDAO;

        public UserManagerService(IHubContext<InformHub> hub, IUserInfoDAO userInfoDAO, ILoginInfoDAO loginInfoDAO, IFriendInfoDAO friendInfoDAO)
        {
            _hub = hub;
            _userInfoDAO = userInfoDAO;
            _loginInfoDAO = loginInfoDAO;
            _friendInfoDAO = friendInfoDAO;
        }

        public bool UserLogin(UserLoginModel data, out LoginInfoModel login, out string message)
        {
            try
            {
                bool state = false;
                login = null;
                message = string.Empty;
                UserInfoModel userInfo = null;
                // 手机号
                if (Regex.IsMatch(data.LoginName, ClassHelper.phoneVerify))
                {
                    if (_userInfoDAO.UserInfoFindForPhoneNumber(data.LoginName, true) is UserInfoModel user)
                    {
                        userInfo = user;
                    }
                    else
                    {
                        message = "手机号尚未注册。";
                    }
                }
                // 邮箱
                else if (Regex.IsMatch(data.LoginName, ClassHelper.emailVerify))
                {
                    if (_userInfoDAO.UserInfoFindForEmail(data.LoginName, true) is UserInfoModel user)
                    {
                        userInfo = user;
                    }
                    else
                    {
                        message = "邮箱尚未注册。";
                    }
                }
                // 昵称
                else
                {
                    if (_userInfoDAO.UserInfoFindForNickName(data.LoginName, true) is UserInfoModel user)
                    {
                        userInfo = user;
                    }
                    else
                    {
                        message = "该用户不存在。";
                    }
                }
                if (userInfo != null)
                {
                    if (string.IsNullOrEmpty(userInfo.Password))
                    {
                        message = "该用户尚未设置密码，请使用免密码登录。";
                    }
                    else
                    {
                        string aesKey = ClassHelper.GenerateSHA256(ClassHelper.TimeStamp(data.LoginTime)).Substring(4, 16).ToUpper(ClassHelper.cultureInfo);
                        if (ClassHelper.AesDecrypt(data.Password, aesKey) != userInfo.Password)
                        {
                            message = "密码错误。";
                        }
                        else
                        {
                            if (_loginInfoDAO.UserLogin(userInfo.UserID, data.UseDevice, data.LoginTime))
                            {
                                login = _loginInfoDAO.ValidLoginInfo(userInfo.UserID, data.UseDevice);
                                state = true;
                                message = "登录成功。";
                            }
                        }
                    }
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetVerificationCode(GetVerificationCodeModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                string code = ClassHelper.GenerateRandomNumber(6);
                if (ClassHelper.SendVerificationCode(data.VerifyAccount, code, out message))
                {
                    lock (ClassHelper.Verifies)
                    {
                        if (ClassHelper.Verifies.Find(item => item.VerifyAccount == data.VerifyAccount && item.UseDevice == data.UseDevice) is VerifyModel verifyModel)
                        {
                            ClassHelper.Verifies.Remove(verifyModel);
                        }
                        ClassHelper.Verifies.Add(new VerifyModel
                        {
                            VerifyAccount = data.VerifyAccount,
                            VerifyCode = code,
                            UseDevice = data.UseDevice,
                            ExpiresTime = DateTime.Now.AddMinutes(5)
                        });
                    }
                    state = true;
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool PhoneNumberLogin(PhoneNumberLoginModel data, out LoginInfoModel login, out string message)
        {
            try
            {
                bool state = false;
                login = null;
                message = string.Empty;
                lock (ClassHelper.Verifies)
                {
                    if (ClassHelper.Verifies.Find(item => item.VerifyAccount == data.PhoneNumber && item.UseDevice == data.UseDevice) is VerifyModel verifyModel)
                    {
                        if (data.VerifyCode == verifyModel.VerifyCode)
                        {
                            if (DateTime.Now > verifyModel.ExpiresTime)
                            {
                                message = "验证码已过期，请重新获取。";
                            }
                            else
                            {
                                if (!_userInfoDAO.PhoneNumberExist(data.PhoneNumber))
                                {
                                    _userInfoDAO.UserInfoAdd(new UserInfoModel
                                    {
                                        UserID = $"{ClassHelper.GetRandomString(10)}-{ClassHelper.GetRandomString(10)}",
                                        NickName = "旅行者",
                                        HeadPortrait = "头像.png",
                                        PhoneNumber = data.PhoneNumber,
                                        Personalized = "系统原装签名，给每一位小可爱~",
                                        CreateTime = DateTime.Now,
                                        UpdateTime = DateTime.Now
                                    });
                                }
                                if (_userInfoDAO.UserInfoFindForPhoneNumber(data.PhoneNumber) is UserInfoModel user)
                                {
                                    if (_loginInfoDAO.UserLogin(user.UserID, data.UseDevice, DateTime.Now))
                                    {
                                        login = _loginInfoDAO.ValidLoginInfo(user.UserID, data.UseDevice);
                                        message = "登录成功。";
                                    }
                                }
                                ClassHelper.Verifies.Remove(verifyModel);
                                state = true;
                            }
                        }
                        else
                        {
                            message = "验证码不正确。";
                        }
                    }
                    else
                    {
                        message = "请先获取验证码。";
                    }
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetUserInfo(string userID, out UserInfoModel user, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                user = _userInfoDAO.UserInfoFindForUserID(userID);
                if (user != null)
                {
                    state = true;
                }
                else
                {
                    message = "该用户不存在。";
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetFriendList(string userID, out List<FriendSortModel> friendSorts, out string message)
        {
            try
            {
                bool state = false;
                friendSorts = new();
                message = string.Empty;
                List<FriendModel> friendModels = _friendInfoDAO.GetFriends(userID);
                List<FriendBriefModel> friendBriefs = new();
                friendBriefs.AddRange(_userInfoDAO.GetUserInfos(friendModels.Select(item => item.UserID).ToList()).Select(item => new FriendBriefModel
                {
                    UserID = item.UserID,
                    NickName = item.NickName,
                    RemarkName = friendModels.Find(friend => friend.UserID == item.UserID).RemarkName,
                    HeadPortrait = item.HeadPortrait,
                    Personalized = item.Personalized,
                    OnLine = item.OnLine
                }));
                foreach (char item in ClassHelper.friendGroup)
                {
                    friendSorts.Add(new FriendSortModel
                    {
                        Sort = item.ToString(),
                        FriendBriefs = friendBriefs.FindAll(delegate (FriendBriefModel friendBrief)
                        {
                            return ClassHelper.PinyinFirst(friendBrief.NickName[0]) == item;
                        })
                    });
                }
                state = true;

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool FriendRegistration(FriendRegistrationModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (!_friendInfoDAO.ConfirmFriend(data.UserID, data.FriendID))
                {
                    _friendInfoDAO.AddFriend(data.UserID, data.FriendID, data.VerifyInfo);
                }
                // 如果直接确认好友关系
                if (_friendInfoDAO.ConfirmFriend(data.UserID, data.FriendID))
                {
                    foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(link => link.UserID == data.UserID || link.UserID == data.FriendID))
                    {
                        string userID = item.UserID == data.UserID ? data.FriendID : data.UserID;
                        _hub.Clients.Client(item.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.FriendChanged.ToString(),
                                                                         arg1: ClassHelper.PinyinFirst(_userInfoDAO.UserInfoFindForUserID(userID).NickName[0]).ToString(),
                                                                         arg2: userID,
                                                                         arg3: true);
                    }
                }
                else
                {
                    foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(link => link.UserID == data.UserID || link.UserID == data.FriendID))
                    {
                        string userID = item.UserID == data.UserID ? data.FriendID : data.UserID;
                        _hub.Clients.Client(item.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.NewFriend.ToString(),
                                                                         arg1: userID);
                    }
                }
                state = true;

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool FriendValidation(FriendValidationModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (!_friendInfoDAO.ConfirmFriend(data.UserID, data.FriendID))
                {
                    _friendInfoDAO.VerifyFriend(data.UserID, data.FriendID, data.Passed);
                    foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(link => link.UserID == data.UserID || link.UserID == data.FriendID))
                    {
                        string userID = item.UserID == data.UserID ? data.FriendID : data.UserID;
                        _hub.Clients.Client(item.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.FriendChanged.ToString(),
                                                                         arg1: ClassHelper.PinyinFirst(_userInfoDAO.UserInfoFindForUserID(userID).NickName[0]).ToString(),
                                                                         arg2: userID,
                                                                         arg3: true);
                    }
                }
                state = true;

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetNewFriendList(string userID, out List<NewFriendBriefModel> newFriendBriefs, out string message)
        {
            try
            {
                bool state = false;
                newFriendBriefs = new();
                message = string.Empty;
                newFriendBriefs = JsonConvert.DeserializeObject<List<NewFriendBriefModel>>(JArray.FromObject(_friendInfoDAO.GetNewFriends(userID)).ToString());
                List<string> userIDs = newFriendBriefs.Select(item => item.UserID).ToList();
                List<UserInfoModel> userInfos = _userInfoDAO.GetUserInfos(userIDs);
                foreach (UserInfoModel userInfo in userInfos)
                {
                    foreach (NewFriendBriefModel newFriend in newFriendBriefs.FindAll(i => i.UserID == userInfo.UserID))
                    {
                        newFriend.NickName = userInfo.NickName;
                        newFriend.HeadPortrait = userInfo.HeadPortrait;
                    };
                }
                newFriendBriefs.Reverse();
                state = true;

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetFriendInfo(GetFriendInfoModel data, out FriendDetailsModel friendDetails, out string message)
        {
            try
            {
                bool state = false;
                friendDetails = new FriendDetailsModel();
                message = string.Empty;
                UserInfoModel userInfoModel = !string.IsNullOrEmpty(data.PhoneNumber)
                    ? _userInfoDAO.UserInfoFindForPhoneNumber(data.PhoneNumber)
                    : _userInfoDAO.UserInfoFindForUserID(data.FriendID);
                if (userInfoModel != null)
                {
                    data.FriendID = userInfoModel.UserID;
                    friendDetails.UserID = userInfoModel.UserID;
                    friendDetails.NickName = userInfoModel.NickName;
                    friendDetails.HeadPortrait = userInfoModel.HeadPortrait;
                    friendDetails.PhoneNumber = userInfoModel.PhoneNumber;
                    friendDetails.Personalized = userInfoModel.Personalized;
                    if (data.UserID == data.FriendID || _friendInfoDAO.ConfirmFriend(data.UserID, data.FriendID))
                    {
                        if (_friendInfoDAO.GetFriends(data.UserID).Find(item => item.UserID == data.FriendID) is FriendModel friend)
                        {
                            friendDetails.RemarkName = friend.RemarkName;
                            friendDetails.RemarkInformation = friend.RemarkInformation;
                        }
                        friendDetails.Email = userInfoModel.Email;
                        friendDetails.Gender = userInfoModel.Gender;
                        friendDetails.Birthday = userInfoModel.Birthday;
                        friendDetails.Location = userInfoModel.Location;
                        friendDetails.Profession = userInfoModel.Profession;
                        friendDetails.OnLine = userInfoModel.OnLine;
                        friendDetails.Friend = true;
                    }
                    state = true;
                }
                else
                {
                    message = "该用户不存在。";
                }

                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UpdateRemarkInfo(UpdateRemarkInfoModel data, out string message)
        {
            try
            {
                bool state = false;
                message = string.Empty;
                if (_friendInfoDAO.ConfirmFriend(data.UserID, data.FriendID))
                {
                    if (data.RemarkName != null || data.RemarkInformation != null)
                    {
                        _friendInfoDAO.UpdateRemark(data.UserID, data.FriendID, data.RemarkName, data.RemarkInformation);
                        foreach (LinkInfoModel item in ClassHelper.LinkInfos.Values.Where(item => item.UserID == data.UserID))
                        {
                            _hub.Clients.Client(item.ConnectionId).SendAsync(method: ClassHelper.HubMessageType.RemarkInfoChanged.ToString(),
                                                                             arg1: data.FriendID);
                        }
                    }
                    state = true;
                }
                else
                {
                    message = "好友关系不存在";
                }
                return state;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
