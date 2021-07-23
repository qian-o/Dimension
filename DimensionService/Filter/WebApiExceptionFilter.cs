using DimensionService.Common;
using DimensionService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DimensionService.Filter
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<WebApiExceptionFilter> _logger;

        public WebApiExceptionFilter(ILogger<WebApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.ErrorLog(context.Exception.TargetSite.Name, context.Exception.StackTrace.ToString(), context.Exception.Message);
            WebResultModel webResult = new()
            {
                State = false,
                Message = context.Exception.Message
            };
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status200OK,
                ContentType = "application/json; charset=utf-8",
                Content = JsonConvert.SerializeObject(webResult)
            };
            context.ExceptionHandled = true;

            base.OnException(context);
        }
    }
}
