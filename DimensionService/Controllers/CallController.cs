using DimensionService.Common;
using DimensionService.Filter.Authorized;
using DimensionService.Models;
using DimensionService.Models.RequestModels;
using DimensionService.Service.Call;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;

namespace DimensionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedActionFilter]
    public class CallController : ControllerBase
    {
        private readonly ICallService _callService;

        public CallController(ICallService callService)
        {
            _callService = callService;
        }

        /// <summary>
        /// 创建通话
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("CreateCall")]
        [HttpPost]
        public WebResultModel CreateCall(CreateCallModel data)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;
            Request.Headers.TryGetValue("Device", out StringValues useDevice);
            data.UseDevice = (ClassHelper.UseDevice)Enum.Parse(typeof(ClassHelper.UseDevice), useDevice);

            WebResultModel webResult = new()
            {
                State = _callService.CreateCall(data, out string roomID, out string message),
                Data = roomID,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取用户UserSig
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        [Route("GetUserSig")]
        [HttpGet]
        public WebResultModel GetUserSig(string roomID)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);

            WebResultModel webResult = new()
            {
                State = _callService.GetUserSig(userID, roomID, out string userSig, out string message),
                Data = userSig,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 通知室友进行通话
        /// </summary>
        /// <returns></returns>
        [Route("NotifyRoommate")]
        [HttpPost]
        public WebResultModel NotifyRoommate()
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            Request.Headers.TryGetValue("Device", out StringValues useDevice);

            WebResultModel webResult = new()
            {
                State = _callService.NotifyRoommate(userID, (ClassHelper.UseDevice)Enum.Parse(typeof(ClassHelper.UseDevice), useDevice), out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 应答通话
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        [Route("ReplyCall")]
        [HttpPost]
        public WebResultModel ReplyCall(ReplyCallModel data)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;

            WebResultModel webResult = new()
            {
                State = _callService.ReplyCall(data, out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 解散房间
        /// </summary>
        /// <returns></returns>
        [Route("DissolutionRoom")]
        [HttpPost]
        public WebResultModel DissolutionRoom()
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            Request.Headers.TryGetValue("Device", out StringValues useDevice);

            WebResultModel webResult = new()
            {
                State = _callService.DissolutionRoom(userID, (ClassHelper.UseDevice)Enum.Parse(typeof(ClassHelper.UseDevice), useDevice), out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }
    }
}