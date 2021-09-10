using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class UpdateRemarkInfoModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "好友ID")]
        [Required(ErrorMessage = "好友ID不能为空")]
        public string FriendID { get; set; }

        [Display(Name = "备注名")]
        public string RemarkName { get; set; }

        [Display(Name = "备注信息")]
        public string RemarkInformation { get; set; }
    }
}
