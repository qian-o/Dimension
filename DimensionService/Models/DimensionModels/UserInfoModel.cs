namespace DimensionService.Models.DimensionModels
{
    public class UserInfoModel
    {
        // 主键
        public int ID { get; set; }
        // 用户ID
        public string UserID { get; set; }
        // 昵称
        public string NickName { get; set; }
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
        // 密码
        public string Password { get; set; }
        // 用户分类 (0 默认  1 管理)
        public int UserType { get; set; }
        // 在线状态
        public bool OnLine { get; set; }
        // 创建时间
        public DateTime CreateTime { get; set; }
        // 更新时间
        public DateTime UpdateTime { get; set; }
    }
}
