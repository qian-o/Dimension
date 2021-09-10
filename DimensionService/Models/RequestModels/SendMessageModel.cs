using DimensionService.Common;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.RequestModels
{
    public class SendMessageModel
    {
        [Display(Name = "聊天ID")]
        [Required(ErrorMessage = "聊天ID不能为空")]
        public string ChatID { get; set; }

        [Display(Name = "用户ID")]
        public string UserID { get; set; }

        [Display(Name = "消息类型")]
        [Required(ErrorMessage = "消息类型不能为空")]
        public ClassHelper.MessageType MessageType { get; set; }

        [Display(Name = "消息内容")]
        [Required(ErrorMessage = "消息内容不能为空")]
        public string MessageContent { get; set; }
    }
}
