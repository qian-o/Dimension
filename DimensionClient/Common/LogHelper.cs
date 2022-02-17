using log4net;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Log4net.config")]
namespace DimensionClient.Common
{
    public static class LogHelper
    {
        /// <summary>
        /// 输出错误日志到Log4Net
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="ex">信息</param>
        public static void WriteErrorLog(Type t, string method, string location, string msg)
        {
            ILog log = LogManager.GetLogger(t);
            log.Error(GetExInfo(method, location, msg));
        }

        /// <summary>
        /// 输出日志信息到Log4Net
        /// </summary>
        /// <param name="t">类型</param>
        /// <param name="ex">信息</param>
        public static void WriteInfoLog(Type t, string method, string location, string msg)
        {
            ILog log = LogManager.GetLogger(t);
            log.Info(GetExInfo(method, location, msg));
        }

        private static string GetExInfo(string method, string location, string msg)
        {
            return new StringBuilder()
                .Append(method.Trim())
                .Append("\r\n位置：")
                .Append("\r\n" + location)
                .Append("\r\n信息：")
                .Append(msg.Trim())
                .ToString();
        }
    }
}
