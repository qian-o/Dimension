using Dimension.Domain;
using DimensionService.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DimensionService.Models.DimensionModels
{
    [Index(nameof(HouseOwnerID), nameof(HouseOwnerDevice))]
    public class CallRoomModel
    {
        // 主键
        public int ID { get; set; }
        // 房主
        public string HouseOwnerID { get; set; }
        // 房主设备类型
        public UseDevice HouseOwnerDevice { get; set; }
        // 房间ID
        public string RoomID { get; set; }
        // 房间通话类型
        public CallType? HouseCallType { get; set; }
        // 室友
        [ConcurrencyCheck]
        public string Roommate { get; set; }
        // 房间是否可用
        [ConcurrencyCheck]
        public bool Enabled { get; set; }
    }
}
