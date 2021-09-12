using DimensionService.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(ChatID))]
    public class ChatMessagesModel
    {
        // 主键
        public int ID { get; set; }
        // 聊天ID
        public string ChatID { get; set; }
        // 发送方ID
        public string SenderID { get; set; }
        // 接收方ID
        public string ReceiverID { get; set; }
        // 接收方是否已读
        [ConcurrencyCheck]
        public bool IsRead { get; set; }
        // 消息类型
        public ClassHelper.MessageType MessageType { get; set; }
        // 消息内容
        public string MessageContent { get; set; }
        // 消息是否可见（ -1全部可不见, 0全部可见, 1发送方不可见, 2接收方不可见 ）
        [ConcurrencyCheck]
        public int IsVisible { get; set; }
        // 消息是否撤回
        [ConcurrencyCheck]
        public bool IsWithdraw { get; set; }
        // 创建时间
        public DateTime CreateTime { get; set; }
    }
}
