using DimensionService.Common;

namespace DimensionService.Models.DimensionModels.FriendInfoModels
{
    public class NewFriendModel
    {
        // 信息类型
        public ClassHelper.NewFriendType FriendType { get; set; }
        // 好友ID
        public string UserID { get; set; }
        // 验证信息
        public string VerifyInfo { get; set; }
        // 是否通过( null 未验证 )
        public bool? Passed { get; set; }
    }
}
