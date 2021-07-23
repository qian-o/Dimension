using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.DimensionModels
{
    public class FriendInfoModel
    {
        // 主键
        public int ID { get; set; }
        // 用户ID
        public string UserID { get; set; }
        // 好友
        [ConcurrencyCheck]
        public string Friends { get; set; }
        // 新好友
        [ConcurrencyCheck]
        public string NewFriends { get; set; }
    }
}
