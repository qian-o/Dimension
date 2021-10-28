using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class GetUserSigModel
    {
        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "房间ID")]
        [Required(ErrorMessage = "房间ID不能为空")]
        public string RoomID { get; set; }
    }
}
