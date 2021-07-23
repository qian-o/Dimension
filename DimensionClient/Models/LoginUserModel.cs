namespace DimensionClient.Models
{
    public class LoginUserModel
    {
        // 主键
        public int ID { get; set; }
        // 用户ID
        public string UserID { get; set; }
        // 昵称
        public string NickName { get; set; }
        // 登录名
        public string LoginName { get; set; }
        // 头像
        public string HeadPortrait { get; set; }
        // 密码
        public string Password { get; set; }
    }
}
