using DimensionClient.Common;
using DimensionClient.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// VideoCallCard.xaml 的交互逻辑
    /// </summary>
    public partial class VideoCallCard : UserControl
    {
        private readonly Storyboard callOpacityShow = ClassHelper.FindResource<Storyboard>("CallOpacity_Show");
        private readonly Storyboard callMainShow = ClassHelper.FindResource<Storyboard>("CallMain_Show");
        private readonly Storyboard callYuyinShow = ClassHelper.FindResource<Storyboard>("CallYuyin_Show");
        private readonly Storyboard callShipinShow = ClassHelper.FindResource<Storyboard>("CallShipin_Show");
        private readonly Storyboard callDianhuaShow = ClassHelper.FindResource<Storyboard>("CallDianhua_Show");
        private readonly Storyboard callOpacityHide = ClassHelper.FindResource<Storyboard>("CallOpacity_Hide");
        private readonly Storyboard callMainHide = ClassHelper.FindResource<Storyboard>("CallMain_Hide");
        private readonly Storyboard callSmallBoxShrink = ClassHelper.FindResource<Storyboard>("CallSmallBox_Shrink");
        private readonly Storyboard callSmallBoxEnlarged = ClassHelper.FindResource<Storyboard>("CallSmallBox_Enlarged");
        private bool isSwitch = false;

        public VideoCallCard()
        {
            InitializeComponent();

            foreach (CallViewDataModel itemCall in ClassHelper.CallViewManager.CallViews)
            {
                if (itemCall.UserID == ClassHelper.UserID)
                {
                    imgSmallBox.SetBinding(Image.SourceProperty, new Binding { Path = new PropertyPath(nameof(itemCall.Writeable)) });
                    imgSmallBox.DataContext = itemCall;
                }
                else
                {
                    imgMainBox.SetBinding(Image.SourceProperty, new Binding { Path = new PropertyPath(nameof(itemCall.Writeable)) });
                    imgMainBox.DataContext = itemCall;
                }
                itemCall.PropertyChanged += ItemCall_PropertyChanged;
            }
        }

        private async void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.AcceptCallSignalR += SignalRClientHelper_AcceptCallSignalR;

            BeginStoryboard(callOpacityShow);
            grdMain.BeginStoryboard(callMainShow);
            await Task.Delay(800);
            stpCallControl.Visibility = Visibility.Visible;
            brdCallYuyin.BeginStoryboard(callYuyinShow);
            brdCallShipin.BeginStoryboard(callShipinShow);
            brdCallDianhua.BeginStoryboard(callDianhuaShow);
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.AcceptCallSignalR -= SignalRClientHelper_AcceptCallSignalR;
        }

        // 是否静音
        private async void BrdCallYuyin_PointerUp(object sender, EventArgs e)
        {
            brdCallYuyin.IsEnabled = false;
            if (brdCallYuyin.Tag.ToString() == "Enable")
            {
                ClassHelper.CallViewManager.MicrophoneSwitch(false);
                brdCallYuyin.Tag = "Disabled";
            }
            else
            {
                ClassHelper.CallViewManager.MicrophoneSwitch(true);
                brdCallYuyin.Tag = "Enable";
            }

            await Task.Delay(1000);
            brdCallYuyin.IsEnabled = true;
        }

        // 是否开启摄像头
        private async void BrdCallShipin_PointerUp(object sender, EventArgs e)
        {
            brdCallShipin.IsEnabled = false;
            if (brdCallShipin.Tag.ToString() == "Enable")
            {
                ClassHelper.CallViewManager.CameraSwitch(false);
                brdCallShipin.Tag = "Disabled";
            }
            else
            {
                ClassHelper.CallViewManager.CameraSwitch(true);
                brdCallShipin.Tag = "Enable";
            }

            await Task.Delay(1000);
            brdCallShipin.IsEnabled = true;
        }

        // 挂断电话
        private void BrdCallDianhua_PointerUp(object sender, EventArgs e)
        {
            ClassHelper.CallViewManager.UnInitialize();
        }

        // 大小画面切换
        private void GrdSmallBox_PointerUp(object sender, EventArgs e)
        {
            if (isSwitch)
            {
                CallViewDataModel callView = imgSmallBox.DataContext as CallViewDataModel;
                imgSmallBox.DataContext = imgMainBox.DataContext;
                imgMainBox.DataContext = callView;
            }
        }

        private void ItemCall_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CallViewDataModel callViewData)
            {
                if (e.PropertyName == nameof(callViewData.IsVideo))
                {
                    Dispatcher.Invoke(delegate
                    {
                        if (!ClassHelper.CallViewManager.CallViews.Any(item => item.IsVideo))
                        {
                            isSwitch = false;
                            imgMainBox.Visibility = Visibility.Collapsed;
                            brdSmallBox.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            brdSmallBox.Visibility = Visibility.Visible;
                            if (!ClassHelper.CallViewManager.CallViews.Any(item => !item.IsVideo))
                            {
                                imgMainBox.Visibility = Visibility.Visible;
                                Lessen();
                            }
                            else
                            {
                                imgMainBox.Visibility = Visibility.Collapsed;
                                if (ClassHelper.CallViewManager.CallViews.FirstOrDefault(item => item.IsVideo) is CallViewDataModel call)
                                {
                                    CallViewDataModel callView = imgSmallBox.DataContext as CallViewDataModel;
                                    if (callView != call)
                                    {
                                        imgSmallBox.DataContext = imgMainBox.DataContext;
                                        imgMainBox.DataContext = callView;
                                    }
                                    Amplification();
                                }
                            }
                        }
                    });
                }
            }
        }

        private void SignalRClientHelper_AcceptCallSignalR(string userID, bool isAcceptCall)
        {
            if (ClassHelper.CallViewManager.CallViews.Any(item => item.UserID == userID))
            {
                Dispatcher.Invoke(delegate
                {
                    if (!isAcceptCall)
                    {
                        ClassHelper.CallViewManager.UnInitialize();
                    }
                });
            }
        }

        #region 执行事件
        private void Lessen()
        {
            isSwitch = true;
            brdSmallBox.Padding = new Thickness(20);
            grdSmallBox.HorizontalAlignment = HorizontalAlignment.Right;
            grdSmallBox.VerticalAlignment = VerticalAlignment.Top;
            imgSmallBox.Stretch = Stretch.Uniform;
            ((imgSmallBox.OpacityMask as VisualBrush).Visual as Border).CornerRadius = new CornerRadius(50);
            grdSmallBox.BeginStoryboard(callSmallBoxShrink);
        }
        private void Amplification()
        {
            isSwitch = false;
            brdSmallBox.Padding = new Thickness(0);
            grdSmallBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            grdSmallBox.VerticalAlignment = VerticalAlignment.Stretch;
            imgSmallBox.Stretch = Stretch.UniformToFill;
            ((imgSmallBox.OpacityMask as VisualBrush).Visual as Border).CornerRadius = new CornerRadius(0);
            grdSmallBox.BeginStoryboard(callSmallBoxEnlarged);
        }
        public async void UnInitializeCard()
        {
            BeginStoryboard(callOpacityHide);
            grdMain.BeginStoryboard(callMainHide);
            await Task.Delay(800);
            Visibility = Visibility.Collapsed;
            if (VisualTreeHelper.GetParent(this) is Grid grid)
            {
                grid.Children.Remove(this);
            }
        }
        #endregion
    }
}
