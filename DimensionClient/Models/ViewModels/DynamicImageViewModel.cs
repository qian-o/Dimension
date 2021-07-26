using DimensionClient.Common;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DimensionClient.Models.ViewModels
{
    public class DynamicImageViewModel : ModelBase
    {
        private bool show1;
        private bool show2;
        private ImageSource source1;
        private ImageSource source2;

        public bool Show1
        {
            get => show1;
            set
            {
                show1 = value;
                OnPropertyChanged(nameof(Show1));
            }
        }
        public bool Show2
        {
            get => show2;
            set
            {
                show2 = value;
                OnPropertyChanged(nameof(Show2));
            }
        }
        public ImageSource Source1
        {
            get => source1;
            set
            {
                source1 = value;
                OnPropertyChanged(nameof(Source1));
            }
        }
        public ImageSource Source2
        {
            get => source2;
            set
            {
                source2 = value;
                OnPropertyChanged(nameof(Source2));
            }
        }

        public override void InitializeVariable()
        {
            show1 = false;
            show2 = true;
            source1 = new BitmapImage(new Uri("/Library/Image/图片.png", UriKind.Relative));
            source2 = new BitmapImage(new Uri("/Library/Image/图片.png", UriKind.Relative));
        }
    }
}