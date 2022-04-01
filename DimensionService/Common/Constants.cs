using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using System.Globalization;

namespace DimensionService.Common
{
    internal static class Constants
    {
        // 手机号正则验证
        public const string PHONE_VERIFY = @"^1[0-9]{10}$";
        // 邮箱正则验证
        public const string EMAIL_VERIFY = @"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";
        // 中英文正则验证
        public const string CHINESE_AND_ENGLISH_VERIFY = @"^[a-zA-Z\u4e00-\u9fa5]$";
        // 中文正则验证
        public const string CHINESE_VERIFY = @"^[\u4e00-\u9fa5]$";
        // 排序
        public const string FRIEND_GROUP = "ABCDEFGHIJKLMNOPQRSTUVWXYZ#";
        public static readonly char[] NUMBERS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        // 区域设置
        public static readonly CultureInfo CurrentCultureInfo = new("zh-cn");
    }
}