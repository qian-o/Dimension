using Microsoft.EntityFrameworkCore;
using System;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(UserID1), nameof(UserID2))]
    public class ChatLinkModel
    {
        // 主键
        public int ID { get; set; }
        // 用户ID 1
        public string UserID1 { get; set; }
        // 用户ID 2
        public string UserID2 { get; set; }
        // 聊天ID
        public string ChatID { get; set; }
    }
}
