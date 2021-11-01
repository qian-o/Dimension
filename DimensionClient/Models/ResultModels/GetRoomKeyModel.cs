namespace DimensionClient.Models.ResultModels
{
    public class GetRoomKeyModel
    {
        // 用户所属的房间权限
        public string UserSig { get; set; }
        // 用户所属的权限票据
        public string PrivateMapKey { get; set; }
    }
}
