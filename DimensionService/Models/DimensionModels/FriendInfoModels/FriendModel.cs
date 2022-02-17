namespace DimensionService.Models.DimensionModels.FriendInfoModels
{
    public class FriendModel
    {
        // 好友ID
        public string UserID { get; set; }
        // 备注名
        public string RemarkName { get; set; }
        // 备注信息
        public string RemarkInformation { get; set; }
        // 成立好友关系时间
        public DateTime FoundTime { get; set; }
    }
}
