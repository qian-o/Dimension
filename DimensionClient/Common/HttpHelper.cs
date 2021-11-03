using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DimensionClient.Common
{
    public static class HttpHelper
    {
        // POST请求
        public static string SendPost(string url, JObject data, bool dimension)
        {
            try
            {
                string result = string.Empty;
                byte[] dataBytes = Encoding.UTF8.GetBytes(data != null ? data.ToString() : string.Empty);
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = dataBytes.Length;
                if (dimension)
                {
                    request.Headers.Add("Service-Version", ClassHelper.serviceVersion);
                    request.Headers.Add("UserID", ClassHelper.UserID);
                    request.Headers.Add("Token", ClassHelper.Token);
                    request.Headers.Add("Device", ClassHelper.device.ToString());
                }
                using Stream writeStream = request.GetRequestStream();
                writeStream.Write(dataBytes, 0, dataBytes.Length);
                using WebResponse response = request.GetResponse();
                if (response != null)
                {
                    using StreamReader reader = new(response.GetResponseStream(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET请求
        public static string SendGet(string url, bool dimension)
        {
            try
            {
                string result = string.Empty;
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                if (dimension)
                {
                    request.Headers.Add("Service-Version", ClassHelper.serviceVersion);
                    request.Headers.Add("UserID", ClassHelper.UserID);
                    request.Headers.Add("Token", ClassHelper.Token);
                    request.Headers.Add("Device", ClassHelper.device.ToString());
                }
                using WebResponse response = request.GetResponse();
                if (response != null)
                {
                    using StreamReader reader = new(response.GetResponseStream(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 上传文件
        public static string SendUpload(string url, MultipartFormDataContent content, bool dimension)
        {
            try
            {
                using HttpClient httpClient = new();
                if (dimension)
                {
                    httpClient.DefaultRequestHeaders.Add("Service-Version", ClassHelper.serviceVersion);
                    httpClient.DefaultRequestHeaders.Add("UserID", ClassHelper.UserID);
                    httpClient.DefaultRequestHeaders.Add("Token", ClassHelper.Token);
                    httpClient.DefaultRequestHeaders.Add("Device", ClassHelper.device.ToString());
                }
                string result = httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
