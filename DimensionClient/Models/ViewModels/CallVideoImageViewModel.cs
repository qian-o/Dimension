using DimensionClient.Common;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DimensionClient.Models.ViewModels
{
    public class CallVideoImageViewModel : ModelBase
    {
        private WriteableBitmap writeable;

        public WriteableBitmap Writeable
        {
            get => writeable;
            set
            {
                writeable = value;
                OnPropertyChanged(nameof(Writeable));
            }
        }
        public Int32Rect Int32Rect { get; set; }

        public override void InitializeVariable()
        {
            writeable = null;
            Int32Rect = new Int32Rect();
        }
    }
}
