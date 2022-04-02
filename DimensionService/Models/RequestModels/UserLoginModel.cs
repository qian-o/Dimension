using Dimension.Domain;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class UserLoginModel
    {
        [Display(Name = "登录名")]
        [Required(ErrorMessage = "登录名不能为空")]
        public string LoginName { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        [Display(Name = "登录设备")]
        [Required(ErrorMessage = "登录设备不能为空")]
        public UseDevice UseDevice { get; set; }

        [Display(Name = "登录时间")]
        [Required(ErrorMessage = "登录时间不能为空")]
        public DateTime LoginTime { get; set; }
    }
}
