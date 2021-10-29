namespace DimensionService.Models.DimensionModels.CallRoomModels
{
    public class RoommateModel
    {
        // 用户ID
        public string UserID { get; set; }
        // 用户所属的房间权限
        public string UserSig { get; set; }
        // 是否进入房间
        public bool? IsEnter { get; set; }
    }
}
