using Dimension.Domain;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class PhoneNumberLoginModel
    {
        [Display(Name = "手机号")]
        [Required(ErrorMessage = "手机号不能为空")]
        public string PhoneNumber { get; set; }

        [Display(Name = "验证码")]
        [Required(ErrorMessage = "验证码不能为空")]
        public string VerifyCode { get; set; }

        [Display(Name = "登录设备")]
        [Required(ErrorMessage = "登录设备不能为空")]
        public UseDevice UseDevice { get; set; }
    }
}
