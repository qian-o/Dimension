using DimensionClient.Common;
using DimensionClient.Library.Controls;
using DimensionClient.Library.CustomControls;
using DimensionClient.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace DimensionClient.Component.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 钩子
        private HwndSource hwnd;

        public MainWindow()
        {
            InitializeComponent();

            ClassHelper.MainWindow = this;
            ClassHelper.MessageHint += ClassHelper_MessageHint;
            ClassHelper.NotificationHint += ClassHelper_NotificationHint;
            ClassHelper.RoutedChanged += ClassHelper_RoutedChanged;
            ClassHelper.AccordingMask += ClassHelper_AccordingMask;
            ClassHelper.DataPassingChanged += ClassHelper_DataPassingChanged;
            ClassHelper.CallChanged += ClassHelper_CallChanged;

            #region 绑定全局属性
            grdInformation.DataContext = ClassHelper.commonView;
            conHeadImage.SetBinding(DynamicImage.ImagePathProperty, new Binding { Path = new PropertyPath("HeadPortrait"), Converter = ClassHelper.FindResource<IValueConverter>("ImageSourceOnlineConvert"), ConverterParameter = "60" });
            conInformationHead.SetBinding(DynamicImage.ImagePathProperty, new Binding { Path = new PropertyPath("HeadPortrait"), Converter = ClassHelper.FindResource<IValueConverter>("ImageSourceOnlineConvert"), ConverterParameter = "60" });
            brdInformationOnLine.SetBinding(Border.BackgroundProperty, new Binding { Path = new PropertyPath("OnLine"), Converter = ClassHelper.FindResource<IValueConverter>("OnLineStatusConvert") });
            txbInformatioNickName.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("NickName") });
            txbInformatioPhoneNumber.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("PhoneNumber") });
            #endregion
        }

        private void AppMain_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Load);
            if (ClassHelper.RegisteredHotkey(this))
            {
                hwnd = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
                hwnd.AddHook(WndProc);
            }
        }

        private void AppMain_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ClassHelper.UnRegisteredHotkey(this))
            {
                hwnd.RemoveHook(WndProc);
            }
        }

        private void AppMain_StateChanged(object sender, EventArgs e)
        {
            // 为什么设置间距设置为4: https://www.cnblogs.com/dino623/p/uielements_of_window.html
            if (WindowState == WindowState.Maximized)
            {
                Thickness thickness = SystemParameters.WindowResizeBorderThickness;
                grdMain.Margin = new Thickness(thickness.Left + 4, thickness.Top + 4, thickness.Right + 4, thickness.Bottom + 4);
            }
            else
            {
                grdMain.Margin = new Thickness(0);
            }
        }

        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnState_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region 切换页面(鼠标,触控)
        private void BrdSelectPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdSelectPage_PointerUp(sender);
            }
        }
        private void BrdSelectPage_TouchUp(object sender, TouchEventArgs e)
        {
            BrdSelectPage_PointerUp(sender);
        }
        #endregion

        private void ClassHelper_MessageHint(int messageType, string message)
        {
            Dispatcher.Invoke(delegate
            {
                ToastMessage toastMessage = new(message, messageType);
                toastMessage.stdLoaded.Completed += (object sender, EventArgs e) =>
                {
                    stpHint.Children.Remove(toastMessage);
                };
                stpHint.Children.Add(toastMessage);
            });
        }

        private void ClassHelper_NotificationHint(string title, string message)
        {
            Dispatcher.Invoke(delegate
            {
                NotificationMessage notificationMessage = new(title, message);
                notificationMessage.storyboard.Completed += (object sender, EventArgs e) =>
                {
                    stpNotification.Children.Remove(notificationMessage);
                };
                stpNotification.Children.Add(notificationMessage);
            });
        }

        private void ClassHelper_RoutedChanged(ClassHelper.PageType pageName)
        {
            Dispatcher.Invoke(delegate
            {
                if (femRouteMain.Content?.GetType().Name == pageName.ToString())
                {
                    return;
                }
                switch (pageName)
                {
                    case ClassHelper.PageType.MessageCenterPage:
                        femRouteMain.Navigate(ClassHelper.messageCenterPage);
                        break;
                    case ClassHelper.PageType.ContactPersonPage:
                        femRouteMain.Navigate(ClassHelper.contactPersonPage);
                        break;
                    default:
                        ClassHelper.MessageAlert(GetType(), 3, ClassHelper.FindResource<string>("PageDoesNotExist"));
                        break;
                }
            });
        }

        private void ClassHelper_AccordingMask(bool show, bool loading)
        {
            Dispatcher.Invoke(delegate
            {
                brdMask.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                thrLoading.Visibility = loading ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void FemRouteMain_Navigated(object sender, NavigationEventArgs e)
        {
            Grid grid = null;
            if (femRouteMain.CanGoBack)
            {
                _ = femRouteMain.RemoveBackEntry();
            }
            switch ((ClassHelper.PageType)Enum.Parse(typeof(ClassHelper.PageType), e.Content.GetType().Name))
            {
                case ClassHelper.PageType.MessageCenterPage:
                    brdMessageCenter.Tag = "1";
                    brdContactPerson.Tag = "0";
                    grdMain.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAFAFA"));
                    grid = brdMessageCenter.Child as Grid;
                    break;
                case ClassHelper.PageType.ContactPersonPage:
                    brdContactPerson.Tag = "1";
                    brdMessageCenter.Tag = "0";
                    grdMain.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                    grid = brdContactPerson.Child as Grid;
                    break;
                default:
                    break;
            }
            if (grid != null)
            {
                ThicknessAnimation thicknessAnimation = new()
                {
                    To = new Thickness(0, Convert.ToInt32(grid.Tag, ClassHelper.cultureInfo), 0, 0),
                    Duration = new TimeSpan(0, 0, 0, 0, 300),
                    EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut }
                };
                brdSlider.BeginAnimation(MarginProperty, thicknessAnimation);
            }
        }

        private void ClassHelper_DataPassingChanged(ClassHelper.DataPassingType dataType, object data)
        {
            if (dataType == ClassHelper.DataPassingType.ScreenCapture)
            {
                ScreenCapture();
            }
        }

        private void ClassHelper_CallChanged(ClassHelper.CallType callType)
        {
            switch (callType)
            {
                case ClassHelper.CallType.Voice:
                    break;
                case ClassHelper.CallType.Video:
                    VideoCall();
                    break;
                case ClassHelper.CallType.ManyVoice:
                    break;
                case ClassHelper.CallType.ManyVideo:
                    break;
                default:
                    break;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == ClassHelper.wmHotKey)
            {
                if (wParam.ToInt32() == ClassHelper.HotKey)
                {
                    foreach (Window item in Application.Current.Windows)
                    {
                        if (item.ShowActivated && item.GetType() == typeof(Screenshots))
                        {
                            return IntPtr.Zero;
                        }
                    }
                    ScreenCapture();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        #region 执行事件
        private void Load(object data)
        {
            ClassHelper.SwitchRoute(ClassHelper.PageType.MessageCenterPage);
        }
        private static void ScreenCapture()
        {
            List<DisplayInfoModel> displays = ClassHelper.GetDisplayInfos();
            int actualLeft = 0;
            int actualTop = 0;
            int actualRight = 0;
            int actualBottom = 0;

            double showLeft = 0;
            double showTop = 0;
            double showRight = 0;
            double showBottom = 0;
            foreach (DisplayInfoModel item in displays)
            {
                if (actualLeft > item.DisplayLeft)
                {
                    actualLeft = item.DisplayLeft;
                }
                if (actualTop > item.DisplayTop)
                {
                    actualTop = item.DisplayTop;
                }
                if (actualRight < item.DisplayLeft + item.DisplayWidth)
                {
                    actualRight = item.DisplayLeft + item.DisplayWidth;
                }
                if (actualBottom < item.DisplayTop + item.DisplayHeight)
                {
                    actualBottom = item.DisplayTop + item.DisplayHeight;
                }

                if (showLeft > item.ShowLeft)
                {
                    showLeft = item.ShowLeft;
                }
                if (showTop > item.ShowTop)
                {
                    showTop = item.ShowTop;
                }
                if (showRight < item.ShowLeft + item.ShowWidth)
                {
                    showRight = item.ShowLeft + item.ShowWidth;
                }
                if (showBottom < item.ShowTop + item.ShowHeight)
                {
                    showBottom = item.ShowTop + item.ShowHeight;
                }
            }
            int actualWidth = Math.Abs(actualLeft) + Math.Abs(actualRight);
            int actualHeight = Math.Abs(actualTop) + Math.Abs(actualBottom);

            double showWidth = Math.Abs(showLeft) + Math.Abs(showRight);
            double showHeight = Math.Abs(showTop) + Math.Abs(showBottom);
            System.Drawing.Bitmap bitmap = new(actualWidth, actualHeight);
            System.Drawing.Graphics.FromImage(bitmap).CopyFromScreen(actualLeft, actualTop, 0, 0, new System.Drawing.Size(actualWidth, actualHeight));

            Screenshots screenshots = new(bitmap, showLeft, showTop, showWidth, showHeight);
            screenshots.ShowDialog();
            if (screenshots.IsSave)
            {
                ClassHelper.TransferringData(typeof(ChatMain), ClassHelper.DataPassingType.Paste, null);
            }

            bitmap.Dispose();
        }
        private static void BrdSelectPage_PointerUp(object sender)
        {
            Border border = (Border)sender;
            ClassHelper.SwitchRoute((ClassHelper.PageType)Enum.Parse(typeof(ClassHelper.PageType), $"{border.Name[3..]}Page"));
        }
        private void VideoCall()
        {
            Dispatcher.Invoke(async delegate
            {
                brdVideoCall.Visibility = Visibility.Visible;
                brdVideoCall.BeginStoryboard(ClassHelper.FindResource<Storyboard>("VideoCallOpacity"));
                brdContent.BeginStoryboard(ClassHelper.FindResource<Storyboard>("VideoCallContent"));
                stpCallControl.Visibility = Visibility.Collapsed;
                await Task.Delay(800);
                stpCallControl.Visibility = Visibility.Visible;
                brdCallYuyin.BeginStoryboard(ClassHelper.FindResource<Storyboard>("VideoCallYuyin"));
                brdCallShipin.BeginStoryboard(ClassHelper.FindResource<Storyboard>("VideoCallShipin"));
                brdCallDianhua.BeginStoryboard(ClassHelper.FindResource<Storyboard>("VideoCallDianhua"));
                if (ClassHelper.CallViewManager.Video.TryGetValue(ClassHelper.UserID, out CallVideoImage videoImage))
                {
                    videoImage.OpacityMask = new VisualBrush(brdSmallBoxMask);
                    grdSmallBox.Children.Add(videoImage);
                }
            });
        }
        #endregion

        private void GrdSmallBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            brdSmallBox.Padding = new Thickness(20);
            grdSmallBox.HorizontalAlignment = HorizontalAlignment.Right;
            grdSmallBox.VerticalAlignment = VerticalAlignment.Top;
            brdSmallBoxMask.CornerRadius = new CornerRadius(50);
            grdSmallBox.BeginStoryboard(ClassHelper.FindResource<Storyboard>("VideoCallSmallBox"));
        }
    }
}
