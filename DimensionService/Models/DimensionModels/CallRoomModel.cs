using DimensionService.Common;
using Microsoft.EntityFrameworkCore;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(HouseOwnerID))]
    public class CallRoomModel
    {
        // 主键
        public int ID { get; set; }
        // 房主
        public string HouseOwnerID { get; set; }
        // 房主设备类型
        public ClassHelper.UseDevice HouseOwnerDevice { get; set; }
        // 房间ID
        public string RoomID { get; set; }
        // 室友
        public string Roommate { get; set; }
        // 房间是否可用
        public bool Enabled { get; set; }
    }
}
