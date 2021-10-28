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
    }
}