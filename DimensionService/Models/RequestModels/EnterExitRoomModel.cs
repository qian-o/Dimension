using DimensionService.Common;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class EnterExitRoomModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "房间ID")]
        [Required(ErrorMessage = "房间ID不能为空")]
        public string RoomID { get; set; }

        [Display(Name = "是否进入")]
        [Required(ErrorMessage = "是否进入不能为空")]
        public bool IsEnter { get; set; }

        [Display(Name = "登录设备")]
        [Required(ErrorMessage = "登录设备不能为空")]
        public ClassHelper.UseDevice UseDevice { get; set; }
    }
}
