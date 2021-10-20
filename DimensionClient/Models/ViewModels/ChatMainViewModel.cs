using DimensionClient.Common;
using System.Collections.Generic;

namespace DimensionClient.Models.ViewModels
{
    public class ChatMainViewModel : ModelBase
    {
        private string messageText;
        private List<EmojiModel> emojis;

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

        public List<EmojiModel> Emojis
        {
            get => emojis;
            set
            {
                emojis = value;
                OnPropertyChanged(nameof(Emojis));
            }
        }

        public override void InitializeVariable()
        {
            ChatID = string.Empty;
            messageText = string.Empty;
            Emojis = null;
        }
    }
}