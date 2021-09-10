using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace DimensionClient.Models.ResultModels
{
    public class ChatColumnInfoModel
    {
        // 好友ID
        public string FriendID { get; set; }
        // 昵称
        public string NickName { get; set; }
        // 备注名
        public string RemarkName { get; set; }
        // 头像
        public string HeadPortrait { get; set; }
        // 聊天ID
        public string ChatID { get; set; }
        // 聊天内容
        public ObservableCollection<ChatMessagesModel> ChatContent { get; set; }
        // 聊天控件
        public ItemsControl Items { get; set; }
    }
}
