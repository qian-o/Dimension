using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using System.Collections.ObjectModel;

namespace DimensionClient.Models.ViewModels
{
    public class ChatMainViewModel : ModelBase
    {
        private string friendNickName;
        private ObservableCollection<ChatMessagesModel> chatContent;
        private string messageText;

        public string ChatID { get; set; }
        public string FriendNickName
        {
            get => friendNickName;
            set
            {
                friendNickName = value;
                OnPropertyChanged(nameof(FriendNickName));
            }
        }
        public ObservableCollection<ChatMessagesModel> ChatContent
        {
            get => chatContent;
            set
            {
                chatContent = value;
                OnPropertyChanged(nameof(ChatContent));
            }
        }

        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged(nameof(MessageText));
            }
        }

        public override void InitializeVariable()
        {
            ChatID = string.Empty;
            friendNickName = string.Empty;
            ChatContent = null;
            messageText = string.Empty;
        }
    }
}