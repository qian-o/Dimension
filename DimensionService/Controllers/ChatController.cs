using DimensionService.Models;
using DimensionService.Models.RequestModels;
using DimensionService.Service.Chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace DimensionService.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [Route("AddChat")]
        [HttpPost]
        public WebResultModel AddChat(AddChatModel data)
        {
            Request.Headers.TryGetValue("UserID", out StringValues userID);
            data.UserID = userID;

            WebResultModel webResult = new()
            {
                State = _chatService.AddChat(data, out string message),
                Data = null,
                Message = message
            };
            return webResult;
        }
    }
}
