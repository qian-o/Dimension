using System;

namespace DimensionClient.Models.ResultModels
{
    public class GetUserInfoModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string NickName { get; set; }
        public string HeadPortrait { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Location { get; set; }
        public string Profession { get; set; }
        public string Personalized { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public bool OnLine { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
