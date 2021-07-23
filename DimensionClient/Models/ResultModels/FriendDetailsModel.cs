using DimensionClient.Common;
using System;

namespace DimensionClient.Models.ResultModels
{
    public class FriendDetailsModel : ModelBase
    {
        private string userID;
        private string nickName;
        private string remarkName;
        private string headPortrait;
        private string phoneNumber;
        private string email;
        private int gender;
        private DateTime? birthday;
        private string location;
        private string profession;
        private string personalized;
        private string remarkInformation;
        private bool onLine;
        private bool friend;

        // 用户ID
        public string UserID
        {
            get => userID;
            set
            {
                userID = value;
                OnPropertyChanged(nameof(UserID));
            }
        }
        // 昵称
        public string NickName
        {
            get => nickName;
            set
            {
                nickName = value;
                OnPropertyChanged(nameof(NickName));
            }
        }
        // 备注名
        public string RemarkName
        {
            get => remarkName;
            set
            {
                remarkName = value;
                OnPropertyChanged(nameof(RemarkName));
            }
        }
        // 头像
        public string HeadPortrait
        {
            get => headPortrait;
            set
            {
                headPortrait = value;
                OnPropertyChanged(nameof(HeadPortrait));
            }
        }
        // 手机号
        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }
        // 邮箱
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        // 性别 (0 默认  1 男  2 女)
        public int Gender
        {
            get => gender;
            set
            {
                gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        // 出生日期
        public DateTime? Birthday
        {
            get => birthday;
            set
            {
                birthday = value;
                OnPropertyChanged(nameof(Birthday));
            }
        }
        // 所在地
        public string Location
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        // 职业
        public string Profession
        {
            get => profession;
            set
            {
                profession = value;
                OnPropertyChanged(nameof(Profession));
            }
        }
        // 个性签名
        public string Personalized
        {
            get => personalized;
            set
            {
                personalized = value;
                OnPropertyChanged(nameof(Personalized));
            }
        }
        // 备注信息
        public string RemarkInformation
        {
            get => remarkInformation;
            set
            {
                remarkInformation = value;
                OnPropertyChanged(nameof(RemarkInformation));
            }
        }
        // 在线状态
        public bool OnLine
        {
            get => onLine;
            set
            {
                onLine = value;
                OnPropertyChanged(nameof(OnLine));
            }
        }
        // 是否为好友
        public bool Friend
        {
            get => friend;
            set
            {
                friend = value;
                OnPropertyChanged(nameof(Friend));
            }
        }

        public override void InitializeVariable()
        {
            userID = string.Empty;
            nickName = string.Empty;
            remarkName = string.Empty;
            headPortrait = string.Empty;
            phoneNumber = string.Empty;
            email = string.Empty;
            gender = 0;
            birthday = null;
            location = string.Empty;
            profession = string.Empty;
            personalized = string.Empty;
            remarkInformation = string.Empty;
            onLine = false;
            friend = false;
        }
    }
}
