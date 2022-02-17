using Newtonsoft.Json.Linq;

namespace DimensionService.Common
{
    public static class LoggingHelper
    {
        public static void ControllerLog(this ILogger logger, string address, string action, JObject result)
        {
            StringWriter stringWriter = new();
            stringWriter.WriteLine($"IP：{address}");
            stringWriter.WriteLine($"接口名：{action}");
            stringWriter.WriteLine($"数据：{result}");
            logger.LogInformation(stringWriter.ToString());
        }

        public static void ErrorLog(this ILogger logger, string method, string stackTrace, string message)
        {
            StringWriter stringWriter = new();
            stringWriter.WriteLine($"方法：{method}");
            stringWriter.WriteLine($"堆栈踪迹：{stackTrace.Trim()}");
            stringWriter.WriteLine($"错误信息：{message}");
            logger.LogError(stringWriter.ToString());
        }
    }
}
