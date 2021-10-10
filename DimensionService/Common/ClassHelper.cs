using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using DimensionService.Models;
using DimensionService.Models.ResultModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.International.Converters.PinYinConverter;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DimensionService.Common
{
    public static class ClassHelper
    {
        #region 常量
        public static readonly char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        // 手机号正则验证
        public const string phoneVerify = @"^1[0-9]{10}$";
        // 邮箱正则验证
        public const string emailVerify = @"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";
        // 中英文正则验证
        public const string ChineseAndEnglishVerify = @"^[a-zA-Z\u4e00-\u9fa5]$";
        // 中文正则验证
        public const string ChineseVerify = @"^[\u4e00-\u9fa5]$";
        // 附件路径
        public static readonly string attachmentsPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Library", "Attachments");
        // 一言句子路径
        public static readonly string sentencesPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Library", "Sentences");
        // 模板路径
        public static readonly string templatesPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Library", "Templates");
        // 区域设置
        public static readonly CultureInfo cultureInfo = new("zh-cn");
        // 排序
        public static readonly string friendGroup = "ABCDEFGHIJKLMNOPQRSTUVWXYZ#";
        // 阿里短信认证信息
        private static readonly IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI4G8MBL2LJo58XoQUBXZJ", "5eAkTpANw59o4up40zv48ac7HKAC2S");
        // 阿里发送短信
        private static readonly DefaultAcsClient client = new(profile);
        #endregion

        #region 变量
        // SignalR连接信息
        public static ConcurrentDictionary<string, LinkInfoModel> LinkInfos { get; set; } = new();
        // 验证码信息
        public static List<VerifyModel> Verifies { get; set; } = new();
        // 一言集合缓存
        public static MemoryCache Cache { get; set; } = new(new MemoryCacheOptions());
        #endregion

        #region 枚举
        // 使用设备
        public enum UseDevice
        {
            Phone,
            Web,
            Client
        }
        // SignalR消息类别
        public enum HubMessageType
        {
            // string title, string message
            Notification,
            // string friendID, bool online
            FriendOnline,
            // string friendID
            NewFriend,
            // string sort, string friendID, bool state ( true 添加 false 删除 )
            FriendChanged,
            // bool online
            OnlineStatus,
            // string friendID
            RemarkInfoChanged,
            // string friendID
            ChatColumnChanged,
            // string chatID
            NewMessage
        }
        // 新朋友类别
        public enum NewFriendType
        {
            Add,
            Verify
        }
        // 消息类别
        public enum MessageType
        {
            Text,
            Voice,
            File,
            VoiceTalk,
            VideoTalk
        }
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
                newRandom.Append(constant[random.Next(10)]);
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
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
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
            //开始加密
            byte[] newBuffer = sHA256.ComputeHash(buffer);
            StringBuilder sb = new();
            for (int i = 0; i < newBuffer.Length; i++)
            {
                sb.Append(newBuffer[i].ToString("x2", cultureInfo));
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
            return ((dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString(cultureInfo);
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
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey) && aesKey.Length == 16)
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
                RijndaelManaged rm = new()
                {
                    Key = Encoding.UTF8.GetBytes(aesKey),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
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
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(aesKey) && aesKey.Length == 16)
            {
                byte[] toEncryptArray = Convert.FromBase64String(str);
                RijndaelManaged rm = new()
                {
                    Key = Encoding.UTF8.GetBytes(aesKey),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                ICryptoTransform cTransform = rm.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                data = Encoding.UTF8.GetString(resultArray);
            }
            return data;
        }

        /// <summary>
        /// 写文件导到磁盘
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static void WriteFile(Stream stream, string path)
        {
            using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            stream.CopyTo(fileStream);
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
            DirectoryInfo folder = new(sentencesPath);
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
            if (Regex.IsMatch(verifyAccount, phoneVerify))
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
            else if (Regex.IsMatch(verifyAccount, emailVerify))
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
            message = string.Empty;
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
            bool state;
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
                    state = false;
                }
            }
            catch (ServerException e)
            {
                message = e.Message;
                state = false;
            }
            catch (ClientException e)
            {
                message = e.Message;
                state = false;
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
            bool state;
            message = string.Empty;
            string path = Path.Combine(templatesPath, "HTML");
            using StreamReader streamReader = new(Path.Combine(path, "verify.html"));
            string contentBody = streamReader.ReadToEnd();
            contentBody = Regex.Replace(contentBody, "验证码位置", code);
            mailMessage.Subject = "验证码";
            mailMessage.Body = contentBody;
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
            if (Regex.IsMatch(str, ChineseAndEnglishVerify))
            {
                if (Regex.IsMatch(str, ChineseVerify))
                {
                    c = ChineseConverter.Convert(str, ChineseConversionDirection.TraditionalToSimplified)[0];
                    ChineseChar chineseChar = new(c);
                    return chineseChar.Pinyins[0][0];
                }
                else
                {
                    return char.ToUpper(c, cultureInfo);
                }
            }
            else
            {
                return '#';
            }
        }
    }
}
