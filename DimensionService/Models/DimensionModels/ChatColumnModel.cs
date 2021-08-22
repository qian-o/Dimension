using Microsoft.EntityFrameworkCore;
using System;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(UserID))]
    public class ChatColumnModel
    {
        // 主键
        public int ID { get; set; }
        // 用户ID
        public string UserID { get; set; }
        // 好友ID
        public string FriendID { get; set; }
        // 聊天ID
        public string ChatID { get; set; }
    }
}
