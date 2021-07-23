using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class FriendRegistrationModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }
        [Display(Name = "好友ID")]
        [Required(ErrorMessage = "好友ID不能为空")]
        public string FriendID { get; set; }
        [Display(Name = "验证信息")]
        [Required(ErrorMessage = "验证信息不能为空")]
        public string VerifyInfo { get; set; }
    }
}
