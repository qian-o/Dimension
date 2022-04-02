using Dimension.Domain;
using DimensionService.Filter.Authorized;
using DimensionService.Models;
using DimensionService.Models.RequestModels;
using DimensionService.Models.ResultModels;
using DimensionService.Service.Call;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
            data.UseDevice = (UseDevice)Enum.Parse(typeof(UseDevice), useDevice);

            WebResultModel webResult = new()
            {
                State = _callService.CreateCall(data, out string roomID, out string message),
                Data = roomID,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取房间Key
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        [Route("GetRoomKey")]
        [HttpGet]
        public WebResultModel GetRoomKey(string roomID)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);

            WebResultModel webResult = new()
            {
                State = _callService.GetRoomKey(userID, roomID, out RoomKeyModel roomKey, out string message),
                Data = roomKey,
                Message = message
            };
            return webResult;
        }

        /// <summary>
        /// 获取房间人员
        /// </summary>
        /// <param name="roomID">房间ID</param>
        /// <returns></returns>
        [Route("GetRoomMember")]
        [HttpGet]
        public WebResultModel GetRoomMember(string roomID)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);

            WebResultModel webResult = new()
            {
                State = _callService.GetRoomMember(userID, roomID, out List<string> member, out string message),
                Data = member,
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
                State = _callService.NotifyRoommate(userID, (UseDevice)Enum.Parse(typeof(UseDevice), useDevice), out string message),
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
            Request.Headers.TryGetValue("Device", out StringValues useDevice);
            data.UseDevice = (UseDevice)Enum.Parse(typeof(UseDevice), useDevice);

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
                State = _callService.DissolutionRoom(userID, (UseDevice)Enum.Parse(typeof(UseDevice), useDevice), out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }
    }
}