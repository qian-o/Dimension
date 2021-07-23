namespace DimensionService.Models.ResultModels
{
    public class FriendBriefModel
    {
        // 用户ID
        public string UserID { get; set; }
        // 昵称
        public string NickName { get; set; }
        // 备注名
        public string RemarkName { get; set; }
        // 头像
        public string HeadPortrait { get; set; }
        // 个性签名
        public string Personalized { get; set; }
        // 在线状态
        public bool OnLine { get; set; }
    }
}
