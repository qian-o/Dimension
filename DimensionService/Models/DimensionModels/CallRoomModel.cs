using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(HouseOwnerID))]
    public class CallRoomModel
    {
        // 主键
        public int ID { get; set; }
        // 房主
        public string HouseOwnerID { get; set; }
        // 房间ID
        public string RoomID { get; set; }
        // 室友
        [ConcurrencyCheck]
        public string Roommate { get; set; }
        // 房间是否可用
        public bool Enabled { get; set; }
    }
}
