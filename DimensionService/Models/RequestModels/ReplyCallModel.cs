using Dimension.Domain;
using DimensionService.Common;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class ReplyCallModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "房间ID")]
        [Required(ErrorMessage = "房间ID不能为空")]
        public string RoomID { get; set; }

        [Display(Name = "是否接受通话")]
        [Required(ErrorMessage = "是否接受通话不能为空")]
        public bool IsAcceptCall { get; set; }

        [Display(Name = "使用设备")]
        public UseDevice UseDevice { get; set; }
    }
}
