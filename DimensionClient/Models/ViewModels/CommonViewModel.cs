using DimensionClient.Common;

namespace DimensionClient.Models.ViewModels
{
    public class CommonViewModel : ModelBase
    {
        private string nickName;
        private string headPortrait;
        private bool onLine;

        public string NickName
        {
            get => nickName;
            set
            {
                nickName = value;
                OnPropertyChanged(nameof(NickName));
            }
        }
        public string HeadPortrait
        {
            get => headPortrait;
            set
            {
                headPortrait = value;
                OnPropertyChanged(nameof(HeadPortrait));
            }
        }
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
            nickName = string.Empty;
            headPortrait = string.Empty;
            onLine = false;
        }
    }
}
