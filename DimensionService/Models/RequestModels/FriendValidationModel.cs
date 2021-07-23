using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class FriendValidationModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }
        [Display(Name = "好友ID")]
        [Required(ErrorMessage = "好友ID不能为空")]
        public string FriendID { get; set; }
        [Display(Name = "是否通过")]
        [Required(ErrorMessage = "是否通过不能为空")]
        public bool Passed { get; set; }
    }
}
