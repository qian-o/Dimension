using DimensionClient.Common;
using System.Collections.Generic;

namespace DimensionClient.Models.ViewModels
{
    public class ChatMainViewModel : ModelBase
    {
        private List<EmojiModel> emojis;

        public string ChatID { get; set; }

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
            Emojis = null;
        }
    }
}