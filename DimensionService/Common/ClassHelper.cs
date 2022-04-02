using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Dimension.Domain;
using DimensionService.Models;
using DimensionService.Models.ResultModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Trtc.V20190722;
using TencentCloud.Trtc.V20190722.Models;

namespace DimensionService.Common
{
    public static partial class ClassHelper
    {

        #region 常量
        // 阿里短信认证信息
        private static readonly IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "", "");
        // 阿里发送短信
        private static readonly DefaultAcsClient client = new(profile);
        // 腾讯API认证ID
        private const string secretID = "";
        // 腾讯API认证Key
        private const string secretKey = "";
        // 通话房间AppID
        public const uint callAppID = 1400587228;
        // 通话房间AppKey
        public const string callAppKey = "";
        // SQL Server连接字符串
        public const string connection = "";
        #endregion

        #region 变量
        // SignalR连接信息
        public static ConcurrentDictionary<string, LinkInfoModel> LinkInfos { get; set; } = new();
        // 验证码信息
        public static List<VerifyModel> Verifies { get; set; } = new();
        // 一言集合缓存
        public static MemoryCache Cache { get; set; } = new(new MemoryCacheOptions());

        #endregion

        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="Length">长度</param>
        /// <returns></returns>
        public static string GenerateRandomNumber(int Length)
        {
            StringBuilder newRandom = new(Length);
            Random random = new();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(Constants.NUMBERS[random.Next(10)]);
            }
            return newRandom.ToString();
        }

        ///<summary>
        /// 随机字符串 
        ///</summary>
        /// <param name="length">长度</param>
        ///<returns></returns>
        public static string GetRandomString(int length)
        {
            byte[] b = new byte[32];
            RandomNumberGenerator.Create().GetBytes(b);
            Random random = new(BitConverter.ToInt32(b, 0));
            string str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string returnStr = string.Empty;
            for (int i = 0; i < length; i++)
            {
                returnStr += str.Substring(random.Next(0, str.Length - 1), 1);
            }
            return returnStr;
        }

        /// <summary>
        /// SHA256转换
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string GenerateSHA256(string str)
        {
            using SHA256 sHA256 = SHA256.Create();
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            byte[] newBuffer = sHA256.ComputeHash(buffer);
            StringBuilder sb = new();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("x2", Constants.CurrentCultureInfo));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string TimeStamp(DateTime dateTime)
        {
            return ((dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString(Constants.CurrentCultureInfo);
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="aesKey">密钥</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string aesKey)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey))
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
                Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                byte[] resultArray = aes.EncryptEcb(toEncryptArray, PaddingMode.PKCS7);
                data = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            return data;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">密文</param>
        /// <param name="aesKey">密钥</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string aesKey)
        {
            string data = string.Empty;
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey))
            {
                byte[] toEncryptArray = Convert.FromBase64String(str);
                Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                byte[] resultArray = aes.DecryptEcb(toEncryptArray, PaddingMode.PKCS7);
                data = Encoding.UTF8.GetString(resultArray);
            }
            return data;
        }

        /// <summary>
        /// 写文件导到磁盘(异步)
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static async Task WriteFileAsync(Stream stream, string path)
        {
            using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            await stream.CopyToAsync(fileStream);
            fileStream.Close();
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="height">图像高度</param>
        /// <returns></returns>
        public static Image CompressPictures(string filePath, int height)
        {
            Image image = Image.Load(filePath);
            if (height < image.Height)
            {
                image.Mutate(item => item.Resize(0, height));
            }
            return image;
        }

        /// <summary>
        /// 获取一言集合
        /// </summary>
        public static void UpdateHitokoto()
        {
            List<HitokotoModel> hitokotos = new();
            DirectoryInfo folder = new(AppPath.SentencesPath);
            foreach (FileInfo file in folder.GetFiles("*.json"))
            {
                using FileStream stream = file.OpenRead();
                JsonElement doc = JsonDocument.Parse(stream).RootElement;
                hitokotos.AddRange(JsonConvert.DeserializeObject<List<HitokotoModel>>(doc.ToString()));
            }
            Console.WriteLine($"更新{hitokotos.Count}条数据");
            Cache.Set("Hitokotos", hitokotos);
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="verifyAccount">验证号码</param>
        /// <param name="code">验证码</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public static bool SendVerificationCode(string verifyAccount, string code, out string message)
        {
            bool state = false;
            if (Regex.IsMatch(verifyAccount, Constants.PHONE_VERIFY))
            {
                if (Debugger.IsAttached)
                {
                    message = code;
                    state = true;
                }
                else
                {
                    state = SendSms(verifyAccount, code, out message);
                }
            }
            else if (Regex.IsMatch(verifyAccount, Constants.EMAIL_VERIFY))
            {
                state = SendMail(verifyAccount, code, out message);
            }
            else
            {
                message = "验证方式仅支持11位手机号或邮箱";
            }
            return state;
        }

        /// <summary>
        /// 发送短信(阿里短信服务)
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <param name="code">验证码</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public static bool SendSms(string phoneNumber, string code, out string message)
        {
            CommonRequest request = new()
            {
                Method = MethodType.POST,
                Domain = "dysmsapi.aliyuncs.com",
                Version = "2017-05-25",
                Action = "SendSms"
            };
            request.AddQueryParameters("PhoneNumbers", phoneNumber);
            request.AddQueryParameters("SignName", "次元社区");
            request.AddQueryParameters("TemplateCode", "SMS_205810832");
            request.AddQueryParameters("TemplateParam", new JObject { { "code", code } }.ToString());
            bool state = false;
            message = string.Empty;
            try
            {
                CommonResponse response = client.GetCommonResponse(request);
                string content = Encoding.Default.GetString(response.HttpResponse.Content);
                SendSmsModel smsModel = JsonConvert.DeserializeObject<SendSmsModel>(content);
                if (smsModel.Code == "OK")
                {
                    state = true;
                }
                else
                {
                    message = smsModel.Message;
                }
            }
            catch (ServerException e)
            {
                message = e.Message;
            }
            catch (ClientException e)
            {
                message = e.Message;
            }
            return state;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="addressee">收件人邮箱</param>
        /// <param name="code">验证码</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public static bool SendMail(string addressee, string code, out string message)
        {
            SmtpClient smtpClient = new()
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp.163.com",
                Credentials = new NetworkCredential("15718810177@163.com", "JTSPMIWTGUJDLBIK")
            };
            MailMessage mailMessage = new()
            {
                From = new MailAddress("15718810177@163.com", "次元社区"),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };
            string path = Path.Combine(AppPath.TemplatesPath, "HTML");
            using StreamReader streamReader = new(Path.Combine(path, "verify.html"));
            string contentBody = streamReader.ReadToEnd();
            contentBody = Regex.Replace(contentBody, "验证码位置", code);
            mailMessage.Subject = "验证码";
            mailMessage.Body = contentBody;
            bool state;
            message = string.Empty;
            try
            {
                mailMessage.To.Add(addressee);
                smtpClient.Send(mailMessage);
                state = true;
            }
            catch (Exception e)
            {
                message = e.Message;
                state = false;
            }
            return state;
        }

        /// <summary>
        /// 返回第一个字的拼音首字母
        /// </summary>
        /// <param name="c">字符串</param>
        /// <returns></returns>
        public static char PinyinFirst(char c)
        {
            string str = c.ToString();
            if (Regex.IsMatch(str, Constants.CHINESE_AND_ENGLISH_VERIFY))
            {
                if (Regex.IsMatch(str, Constants.CHINESE_VERIFY))
                {
                    c = ChineseConverter.Convert(str, ChineseConversionDirection.TraditionalToSimplified)[0];
                    ChineseChar chineseChar = new(c);
                    return chineseChar.Pinyins[0][0];
                }
                else
                {
                    return char.ToUpper(c, Constants.CurrentCultureInfo);
                }
            }
            else
            {
                return '#';
            }
        }

        /// <summary>
        /// 解散房间
        /// </summary>
        /// <param name="sdkAppId">应用服务ID</param>
        /// <param name="roomID">房间ID</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public static bool DissolutionRoom(uint sdkAppId, string roomID, out string message)
        {
            bool state = false;
            message = string.Empty;
            try
            {
                ClientProfile clientProfile = new()
                {
                    HttpProfile = new HttpProfile() { Endpoint = "trtc.tencentcloudapi.com" }
                };
                TrtcClient client = new(new Credential() { SecretId = secretID, SecretKey = secretKey }, "ap-beijing", clientProfile);
                DismissRoomByStrRoomIdRequest request = new()
                {
                    SdkAppId = Convert.ToUInt64(sdkAppId),
                    RoomId = roomID
                };
                client.DismissRoomByStrRoomIdSync(request);
                state = true;
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return state;
        }

        /// <summary>
        /// 获取通话服务权限
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="roomID">房间ID</param>
        /// <param name="callType">通话类型</param>
        /// <param name="privateMapKey">权限票据</param>
        /// <param name="createRoom">是否创建房间</param>
        /// <returns></returns>
        public static string GetCallAuthorization(string userID, string roomID, CallType callType, out string privateMapKey, bool createRoom = false)
        {
            TLSSigAPIv2 aPIv2 = new(callAppID, callAppKey);
            privateMapKey = aPIv2.GenPrivateMapKeyWithStringRoomID(userID, 43200, roomID, createRoom
                ? callType is CallType.Video or CallType.ManyVideo ? 255 : (uint)15
                : callType is CallType.Video or CallType.ManyVideo ? 254 : (uint)14);
            return aPIv2.GenUserSig(userID, 43200);
        }
    }
}
