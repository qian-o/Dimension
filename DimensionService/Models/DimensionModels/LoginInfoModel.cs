using DimensionService.Common;
using Microsoft.EntityFrameworkCore;
using System;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(UserID))]
    public class LoginInfoModel
    {
        // 主键
        public int ID { get; set; }
        // 用户ID
        public string UserID { get; set; }
        // 令牌
        public string Token { get; set; }
        // 是否有效
        public bool Effective { get; set; }
        // 登录设备
        public ClassHelper.UseDevice UseDevice { get; set; }
        // 创建时间
        public DateTime CreateTime { get; set; }
        // 更新时间
        public DateTime UpdateTime { get; set; }
    }
}
