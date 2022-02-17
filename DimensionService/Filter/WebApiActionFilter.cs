using DimensionService.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace DimensionService.Filter
{
    /// <summary>
    /// 全局接口拦截器
    /// </summary>
    public class WebApiActionFilter : ActionFilterAttribute
    {
        private readonly ILogger<WebApiActionFilter> _logger;

        public WebApiActionFilter(ILogger<WebApiActionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.ControllerLog(context.HttpContext.Connection.RemoteIpAddress.ToString(), $"{context.ActionDescriptor.RouteValues["controller"]}{context.ActionDescriptor.RouteValues["action"]}", JObject.FromObject(context.ActionArguments));

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
