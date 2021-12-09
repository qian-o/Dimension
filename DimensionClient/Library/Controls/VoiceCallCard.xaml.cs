using DimensionClient.Common;
using DimensionClient.Models;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// VoiceCallCard.xaml 的交互逻辑
    /// </summary>
    public partial class VoiceCallCard : UserControl
    {
        private readonly Storyboard callOpacityShow = ClassHelper.FindResource<Storyboard>("CallOpacity_Show");
        private readonly Storyboard callMainShow = ClassHelper.FindResource<Storyboard>("CallMain_Show");
        private readonly Storyboard callYuyinShow = ClassHelper.FindResource<Storyboard>("CallYuyin_Show");
        private readonly Storyboard callDianhuaShow = ClassHelper.FindResource<Storyboard>("CallDianhua_Show");
        private readonly Storyboard voiceCallAnswer = ClassHelper.FindResource<Storyboard>("VoiceCallAnswer");
        private readonly Storyboard voiceCallFriendAnswer = ClassHelper.FindResource<Storyboard>("VoiceCallFriendAnswer");
        private readonly Storyboard voiceCallReject = ClassHelper.FindResource<Storyboard>("VoiceCallReject");
        private readonly Storyboard callOpacityHide = ClassHelper.FindResource<Storyboard>("CallOpacity_Hide");
        private readonly Storyboard callMainHide = ClassHelper.FindResource<Storyboard>("CallMain_Hide");

        public VoiceCallCard()
        {
            InitializeComponent();
        }

        private async void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.AcceptCallSignalR += SignalRClientHelper_AcceptCallSignalR;

            if (ClassHelper.CallViewManager.CallViews.FirstOrDefault(item => item.UserID == ClassHelper.UserID) is CallViewDataModel callViewUser)
            {
                imgHeadImage.DataContext = callViewUser.UserID;
            }
            if (ClassHelper.CallViewManager.CallViews.FirstOrDefault(item => item.UserID != ClassHelper.UserID) is CallViewDataModel callViewFriend)
            {
                if (callViewFriend.IsEnter == true)
                {
                    Answer();
                }
                imgFriend.DataContext = callViewFriend.UserID;
                callViewFriend.PropertyChanged += CallViewFriend_PropertyChanged;
            }

            BeginStoryboard(callOpacityShow);
            grdMain.BeginStoryboard(callMainShow);
            await Task.Delay(800);
            stpCallControl.Visibility = Visibility.Visible;
            brdCallYuyin.BeginStoryboard(callYuyinShow);
            brdCallDianhua.BeginStoryboard(callDianhuaShow);
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.AcceptCallSignalR -= SignalRClientHelper_AcceptCallSignalR;
        }

        #region 麦克风开关(鼠标,触控)
        private void BrdCallYuyin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdCallYuyin_PointerUp();
            }
        }
        private void BrdCallYuyin_TouchUp(object sender, TouchEventArgs e)
        {
            BrdCallYuyin_PointerUp();
        }
        #endregion

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

        private void CallViewFriend_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is CallViewDataModel callViewData)
            {
                if (callViewData.IsEnter == true)
                {
                    Dispatcher.Invoke(() => Answer());
                }
            }
        }

        private void SignalRClientHelper_AcceptCallSignalR(string userID, bool isAcceptCall)
        {
            if (ClassHelper.CallViewManager.CallViews.Any(item => item.UserID == userID))
            {
                Dispatcher.Invoke(async delegate
                {
                    if (isAcceptCall)
                    {
                        Answer();
                    }
                    else
                    {
                        BeginStoryboard(voiceCallReject);
                        await Task.Delay(500);
                        UnInitializeCard();
                    }
                });
            }
        }

        #region 执行事件
        private async void BrdCallYuyin_PointerUp()
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
        private static void BrdCallDianhua_PointerUp()
        {
            ClassHelper.CallViewManager.UnInitialize();
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
        private void Answer()
        {
            BeginStoryboard(voiceCallAnswer);
            imgFriend.BeginStoryboard(voiceCallFriendAnswer);
        }
        #endregion
    }
}
