using Dimension.Domain;
using DimensionService.Dao.LoginInfo;
using DimensionService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DimensionService.Filter.Authorized
{
    public class AuthorizedActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Debugger.IsAttached)
            {
                string action = context.ActionDescriptor.RouteValues["action"];
                if (action is not "UserLogin" and not "GetVerificationCode" and not "PhoneNumberLogin" and not "GetAttachments" and not "GetHeadPortraits")
                {
                    WebResultModel webResult = new()
                    {
                        State = false
                    };
                    if (context.HttpContext.Request.Headers.TryGetValue("UserID", out StringValues userID) && context.HttpContext.Request.Headers.TryGetValue("Token", out StringValues token) && context.HttpContext.Request.Headers.TryGetValue("Device", out StringValues useDevice))
                    {
                        if (!LoginInfoDAO.CheckToken(userID, token, (UseDevice)Enum.Parse(typeof(UseDevice), useDevice)))
                        {
                            webResult.Message = "登录已失效";
                        }
                    }
                    else
                    {
                        webResult.Message = "不规范请求";
                    }
                    if (!string.IsNullOrEmpty(webResult.Message))
                    {
                        context.Result = new ContentResult
                        {
                            StatusCode = StatusCodes.Status200OK,
                            ContentType = "application/json; charset=utf-8",
                            Content = JsonConvert.SerializeObject(webResult)
                        };
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
