using DimensionClient.Common;

namespace DimensionClient.Models.ViewModels
{
    public class ChatMainViewModel : ModelBase
    {
        private string friendNickName;
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
            messageText = string.Empty;
        }
    }
}