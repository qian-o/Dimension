using DimensionService.Common;

namespace DimensionService.Models
{
    public class VerifyModel
    {
        // 验证号码
        public string VerifyAccount { get; set; }
        // 验证码
        public string VerifyCode { get; set; }
        // 使用设备
        public ClassHelper.UseDevice UseDevice { get; set; }
        // 过期时间
        public DateTime ExpiresTime { get; set; }
    }
}
