using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class AddChatModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "好友ID")]
        [Required(ErrorMessage = "好友ID不能为空")]
        public string FriendID { get; set; }
    }
}
