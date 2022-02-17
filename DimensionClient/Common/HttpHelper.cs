using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace DimensionClient.Common
{
    public static class HttpHelper
    {
        // POST请求
        public static async Task<string> SendPost(string url, JObject data, bool dimension)
        {
            try
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(data != null ? data.ToString() : string.Empty);
                HttpClient request = new();
                if (dimension)
                {
                    request.DefaultRequestHeaders.Add("Service-Version", ClassHelper.serviceVersion);
                    request.DefaultRequestHeaders.Add("UserID", ClassHelper.UserID);
                    request.DefaultRequestHeaders.Add("Token", ClassHelper.Token);
                    request.DefaultRequestHeaders.Add("Device", ClassHelper.device.ToString());
                }
                ByteArrayContent jsonContent = new(Encoding.UTF8.GetBytes(data != null ? data.ToString() : string.Empty));
                jsonContent.Headers.Add("Content-Type", "application/json");
                return await (await request.PostAsync(url, jsonContent)).Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET请求
        public static async Task<string> SendGet(string url, bool dimension)
        {
            try
            {
                HttpClient httpClient = new();
                if (dimension)
                {
                    httpClient.DefaultRequestHeaders.Add("Service-Version", ClassHelper.serviceVersion);
                    httpClient.DefaultRequestHeaders.Add("UserID", ClassHelper.UserID);
                    httpClient.DefaultRequestHeaders.Add("Token", ClassHelper.Token);
                    httpClient.DefaultRequestHeaders.Add("Device", ClassHelper.device.ToString());
                }
                return await (await httpClient.GetAsync(url)).Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 上传文件
        public static async Task<string> SendUpload(string url, MultipartFormDataContent content, bool dimension)
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
                return await (await httpClient.PostAsync(url, content)).Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
