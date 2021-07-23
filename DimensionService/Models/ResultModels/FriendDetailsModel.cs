using System;

namespace DimensionService.Models.ResultModels
{
    public class FriendDetailsModel
    {
        // 用户ID
        public string UserID { get; set; }
        // 昵称
        public string NickName { get; set; }
        // 备注名
        public string RemarkName { get; set; }
        // 头像
        public string HeadPortrait { get; set; }
        // 手机号
        public string PhoneNumber { get; set; }
        // 邮箱
        public string Email { get; set; }
        // 性别 (0 默认  1 男  2 女)
        public int Gender { get; set; }
        // 出生日期
        public DateTime? Birthday { get; set; }
        // 所在地
        public string Location { get; set; }
        // 职业
        public string Profession { get; set; }
        // 个性签名
        public string Personalized { get; set; }
        // 备注信息
        public string RemarkInformation { get; set; }
        // 在线状态
        public bool OnLine { get; set; }
        // 是否为好友
        public bool Friend { get; set; }
    }
}
