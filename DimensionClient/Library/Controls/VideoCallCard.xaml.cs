using DimensionClient.Common;
using DimensionClient.Models;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// VideoCallCard.xaml 的交互逻辑
    /// </summary>
    public partial class VideoCallCard : UserControl
    {
        private readonly Storyboard videoCallOpacityShow = ClassHelper.FindResource<Storyboard>("VideoCallOpacity_Show");
        private readonly Storyboard videoCallMainShow = ClassHelper.FindResource<Storyboard>("VideoCallMain_Show");
        private readonly Storyboard videoCallYuyinShow = ClassHelper.FindResource<Storyboard>("VideoCallYuyin_Show");
        private readonly Storyboard videoCallShipinShow = ClassHelper.FindResource<Storyboard>("VideoCallShipin_Show");
        private readonly Storyboard videoCallDianhuaShow = ClassHelper.FindResource<Storyboard>("VideoCallDianhua_Show");
        private readonly Storyboard videoCallOpacityHide = ClassHelper.FindResource<Storyboard>("VideoCallOpacity_Hide");
        private readonly Storyboard videoCallMainHide = ClassHelper.FindResource<Storyboard>("VideoCallMain_Hide");
        private readonly Storyboard videoCallSmallBoxShrink = ClassHelper.FindResource<Storyboard>("VideoCallSmallBox_Shrink");

        public VideoCallCard()
        {
            InitializeComponent();
        }

        private async void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            BeginStoryboard(videoCallOpacityShow);
            grdMain.BeginStoryboard(videoCallMainShow);
            stpCallControl.Visibility = Visibility.Collapsed;
            await Task.Delay(800);
            stpCallControl.Visibility = Visibility.Visible;
            brdCallYuyin.BeginStoryboard(videoCallYuyinShow);
            brdCallShipin.BeginStoryboard(videoCallShipinShow);
            brdCallDianhua.BeginStoryboard(videoCallDianhuaShow);
            foreach (CallVideoDataModel item in ClassHelper.CallViewManager.Video)
            {
                if (item.UserID == ClassHelper.UserID)
                {
                    imgSmallBox.SetBinding(Image.SourceProperty, new Binding { Source = item, Path = new PropertyPath(nameof(item.Writeable)) });
                }
                else
                {
                    if (item.Writeable != null)
                    {
                        Lessen();
                    }
                    else
                    {
                        item.PropertyChanged += CallVideoData_PropertyChanged;
                    }
                    imgMainBox.SetBinding(Image.SourceProperty, new Binding { Source = item, Path = new PropertyPath(nameof(item.Writeable)) });
                }
            }
        }

        #region 挂断电话(鼠标,触控)
        private void BrdCallDianhua_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdCallDianhua_PointerUp();
            }
        }
        private void BrdCallDianhua_TouchUp(object sender, TouchEventArgs e)
        {
            BrdCallDianhua_PointerUp();
        }
        #endregion

        private void CallVideoData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Writeable")
            {
                Lessen();
            }
        }

        #region 执行事件
        private static void BrdCallDianhua_PointerUp()
        {
            ClassHelper.CallViewManager.UnInitialize();
        }
        public async void UnInitializeCard()
        {
            BeginStoryboard(videoCallOpacityHide);
            grdMain.BeginStoryboard(videoCallMainHide);
            await Task.Delay(800);
            Visibility = Visibility.Collapsed;
            if (VisualTreeHelper.GetParent(this) is Grid grid)
            {
                grid.Children.Remove(this);
            }
        }
        private void Lessen()
        {
            brdSmallBox.Padding = new Thickness(20);
            grdSmallBox.HorizontalAlignment = HorizontalAlignment.Right;
            grdSmallBox.VerticalAlignment = VerticalAlignment.Top;
            imgSmallBox.Stretch = Stretch.Uniform;
            ((imgSmallBox.OpacityMask as VisualBrush).Visual as Border).CornerRadius = new CornerRadius(50);
            grdSmallBox.BeginStoryboard(videoCallSmallBoxShrink);
        }
        #endregion
    }
}
