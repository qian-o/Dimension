namespace DimensionService.Models.DimensionModels.CallRoomModels
{
    public class RoommateModel
    {
        // 用户ID
        public string UserID { get; set; }
        // 用户所属的房间权限
        public string UserSig { get; set; }
        // 是否进屋
        public bool EnterRoom { get; set; }
    }
}
