using DimensionService.Common;
using DimensionService.Dao.LoginInfo;
using Microsoft.Extensions.Primitives;

namespace DimensionService.Middleware
{
    public class SignalRQueryStringAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoginInfoDAO _loginInfoDAO;

        public SignalRQueryStringAuthMiddleware(RequestDelegate next, ILoginInfoDAO loginInfoDAO)
        {
            _next = next;
            _loginInfoDAO = loginInfoDAO;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers["Connection"] == "Upgrade")
            {
                if (context.Request.Query.TryGetValue("UserID", out StringValues userID) && context.Request.Query.TryGetValue("Token", out StringValues token) && context.Request.Query.TryGetValue("Device", out StringValues useDevice))
                {
                    if (!_loginInfoDAO.CheckToken(userID, token, (ClassHelper.UseDevice)Enum.Parse(typeof(ClassHelper.UseDevice), useDevice)))
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            await _next.Invoke(context);
        }
    }
}
