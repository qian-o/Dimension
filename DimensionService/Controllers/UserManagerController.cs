using DimensionService.Filter.UserManager;
using DimensionService.Models;
using DimensionService.Models.DimensionModels;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using DimensionService.Service.UserManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace DimensionService.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [UserManagerActionFilter]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserManagerService _userManager;

        public UserManagerController(IUserManagerService userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("UserLogin")]
        [HttpPost]
        public WebResultModel UserLogin(UserLoginModel data)
        {
            WebResultModel webResult = new()
            {
                State = _userManager.UserLogin(data, out LoginInfoModel login, out string message),
                Data = login,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("GetVerificationCode")]
        [HttpPost]
        public WebResultModel GetVerificationCode(GetVerificationCodeModel data)
        {
            WebResultModel webResult = new()
            {
                State = _userManager.GetVerificationCode(data, out string message),
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 手机号登录
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("PhoneNumberLogin")]
        [HttpPost]
        public WebResultModel PhoneNumberLogin(PhoneNumberLoginModel data)
        {
            WebResultModel webResult = new()
            {
                State = _userManager.PhoneNumberLogin(data, out LoginInfoModel login, out string message),
                Data = login,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Route("GetUserInfo")]
        [HttpGet]
        public WebResultModel GetUserInfo()
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            WebResultModel webResult = new()
            {
                State = _userManager.GetUserInfo(userID, out UserInfoModel user, out string message),
                Data = user,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        [Route("GetFriendList")]
        [HttpGet]
        public WebResultModel GetFriendList()
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            WebResultModel webResult = new()
            {
                State = _userManager.GetFriendList(userID, out List<FriendSortModel> friendSorts, out string message),
                Data = friendSorts,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("FriendRegistration")]
        [HttpPost]
        public WebResultModel FriendRegistration(FriendRegistrationModel data)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;

            WebResultModel webResult = new()
            {
                State = _userManager.FriendRegistration(data, out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 验证好友
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("FriendValidation")]
        [HttpPost]
        public WebResultModel FriendValidation(FriendValidationModel data)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;

            WebResultModel webResult = new()
            {
                State = _userManager.FriendValidation(data, out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取新朋友列表
        /// </summary>
        /// <returns></returns>
        [Route("GetNewFriendList")]
        [HttpGet]
        public WebResultModel GetNewFriendList()
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            WebResultModel webResult = new()
            {
                State = _userManager.GetNewFriendList(userID, out List<NewFriendBriefModel> newFriendBriefs, out string message),
                Data = newFriendBriefs,
                Message = message
            };
            return webResult;

        }

        /// <summary>
        /// 获取好友详细信息
        /// </summary>
        /// <param name="friendID">好友ID</param>
        /// <param name="phoneNumber">手机号</param>
        /// <returns></returns>
        [Route("GetFriendInfo")]
        [HttpGet]
        public WebResultModel GetFriendInfo(string friendID, string phoneNumber)
        {
            GetFriendInfoModel data = new()
            {
                FriendID = friendID,
                PhoneNumber = phoneNumber
            };
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;

            WebResultModel webResult = new()
            {
                State = _userManager.GetFriendInfo(data, out FriendDetailsModel friendDetails, out string message),
                Data = friendDetails,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 更新好友备注
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("UpdateRemarkInfo")]
        [HttpPost]
        public WebResultModel UpdateRemarkInfo(UpdateRemarkInfoModel data)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;

            WebResultModel webResult = new()
            {
                State = _userManager.UpdateRemarkInfo(data, out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }
    }
}
