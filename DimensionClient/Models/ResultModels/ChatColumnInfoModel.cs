using DimensionClient.Common;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace DimensionClient.Models.ResultModels
{
    public class ChatColumnInfoModel : ModelBase
    {
        private string nickName;
        private string remarkName;
        private string headPortrait;
        private ObservableCollection<ChatMessagesModel> chatContent;
        private int unread;
        private bool isUsable;

        // 好友ID
        public string FriendID { get; set; }
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
        // 聊天ID
        public string ChatID { get; set; }
        // 聊天内容
        public ObservableCollection<ChatMessagesModel> ChatContent
        {
            get => chatContent;
            set
            {
                chatContent = value;
                OnPropertyChanged(nameof(ChatContent));
            }
        }
        // 未读数
        public int Unread
        {
            get => unread;
            set
            {
                unread = value;
                OnPropertyChanged(nameof(Unread));
            }
        }
        // 富文本内容
        public FlowDocument Flow { get; set; }
        // 发送中
        public bool IsUsable
        {
            get => isUsable;
            set
            {
                isUsable = value;
                OnPropertyChanged(nameof(IsUsable));
            }
        }

        public override void InitializeVariable()
        {
            nickName = string.Empty;
            remarkName = string.Empty;
            headPortrait = string.Empty;
            chatContent = null;
            unread = 0;
            Flow = null;
            isUsable = true;
        }
    }
}
