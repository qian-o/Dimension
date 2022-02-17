using Newtonsoft.Json.Linq;

namespace DimensionService.Common
{
    public static class LoggingHelper
    {
        public static void ControllerLog(this ILogger logger, string method, string controller, string action, JObject result)
        {
            StringWriter stringWriter = new();
            stringWriter.WriteLine($"方法：{method}");
            stringWriter.WriteLine($"控制器：{controller}");
            stringWriter.WriteLine($"接口：{action}");
            stringWriter.WriteLine($"数据：{result}");
            logger.LogInformation(stringWriter.ToString());
        }

        public static void ErrorLog(this ILogger logger, string method, string stackTrace, string message)
        {
            StringWriter stringWriter = new();
            stringWriter.WriteLine($"方法：{method}");
            stringWriter.WriteLine($"堆栈踪迹：{stackTrace}");
            stringWriter.WriteLine($"错误信息：{message}");
            logger.LogInformation(stringWriter.ToString());
        }
    }
}
