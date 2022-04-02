using Dimension.Domain;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class GetVerificationCodeModel
    {
        [Display(Name = "验证号码")]
        [Required(ErrorMessage = "验证号码不能为空")]
        public string VerifyAccount { get; set; }

        [Display(Name = "使用设备")]
        [Required(ErrorMessage = "使用设备不能为空")]
        public UseDevice UseDevice { get; set; }
    }
}
