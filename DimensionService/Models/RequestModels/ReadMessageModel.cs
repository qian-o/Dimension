using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class ReadMessageModel
    {
        [Display(Name = "聊天ID")]
        [Required(ErrorMessage = "聊天ID不能为空")]
        public string ChatID { get; set; }

        [Display(Name = "消息ID")]
        [Required(ErrorMessage = "消息ID不能为空")]
        public int MessageID { get; set; }

        [Display(Name = "用户ID")]
        public string UserID { get; set; }
    }
}
