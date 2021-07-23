using DimensionService.Common;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class GetVerificationCodeModel
    {
        [Display(Name = "验证号码")]
        public string VerifyAccount { get; set; }

        [Display(Name = "使用设备")]
        public ClassHelper.UseDevice UseDevice { get; set; }
    }
}
