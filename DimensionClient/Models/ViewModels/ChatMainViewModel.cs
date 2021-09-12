using DimensionClient.Common;

namespace DimensionClient.Models.ViewModels
{
    public class ChatMainViewModel : ModelBase
    {
        private string messageText;

        public string ChatID { get; set; }

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
            messageText = string.Empty;
        }
    }
}