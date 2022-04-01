using Dimension.Domain;
using DimensionClient.Common;

namespace DimensionClient.Models.ResultModels
{
    public class UserLoginModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Token { get; set; }
        public bool Effective { get; set; }
        public UseDevice UseDevice { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
