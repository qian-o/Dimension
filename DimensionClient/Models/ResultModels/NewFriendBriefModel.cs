using Dimension.Domain;
using DimensionClient.Common;

namespace DimensionClient.Models.ResultModels
{
    public class NewFriendBriefModel
    {
        // 信息类型
        public NewFriendType FriendType { get; set; }
        // 好友ID
        public string UserID { get; set; }
        // 昵称
        public string NickName { get; set; }
        // 头像
        public string HeadPortrait { get; set; }
        // 验证信息
        public string VerifyInfo { get; set; }
        // 是否通过( null 未验证 )
        public bool? Passed { get; set; }
    }
}
