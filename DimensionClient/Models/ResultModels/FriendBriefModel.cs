using DimensionClient.Common;

namespace DimensionClient.Models.ResultModels
{
    public class FriendBriefModel : ModelBase
    {
        private string userID;
        private string nickName;
        private string remarkName;
        private string headPortrait;
        private string personalized;
        private bool onLine;

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

        public override void InitializeVariable()
        {
            userID = string.Empty;
            nickName = string.Empty;
            remarkName = string.Empty;
            headPortrait = string.Empty;
            personalized = string.Empty;
            onLine = false;
        }
    }
}
