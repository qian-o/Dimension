using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.UserManager;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Component.Pages
{
    /// <summary>
    /// ContactPersonPage.xaml 的交互逻辑
    /// </summary>
    public partial class ContactPersonPage : Page
    {
        private readonly ContactPersonViewModel mainData;
        private readonly FriendDetailsModel newFriendData;

        public ContactPersonPage()
        {
            InitializeComponent();

            mainData = Resources["MainData"] as ContactPersonViewModel;
            newFriendData = Resources["NewFriendData"] as FriendDetailsModel;
            SignalRClientHelper.FriendOnlineSignalR += SignalRClientHelper_FriendOnlineSignalR;
            SignalRClientHelper.NewFriendSignalR += SignalRClientHelper_NewFriendSignalR;
            SignalRClientHelper.FriendChangedSignalR += SignalRClientHelper_FriendChangedSignalR;
            SignalRClientHelper.RemarkInfoChangedSignalR += SignalRClientHelper_RemarkInfoChangedSignalR;
        }

        private void ContactPersonMain_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Load);
        }

        #region 添加好友(鼠标,触控)
        private void TxbAddFriends_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbAddFriends_PointerUp();
            }
        }
        private void TxbAddFriends_TouchUp(object sender, TouchEventArgs e)
        {
            TxbAddFriends_PointerUp();
        }
        #endregion

        #region 关闭弹窗(鼠标,触控)
        private void TxbPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbPopupClose_PointerUp();
            }
        }
        private void TxbPopupClose_TouchUp(object sender, TouchEventArgs e)
        {
            TxbPopupClose_PointerUp();
        }
        #endregion

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(newFriendData.PhoneNumber))
            {
                if (Regex.IsMatch(newFriendData.PhoneNumber, ClassHelper.phoneVerify))
                {
                    ThreadPool.QueueUserWorkItem(FindFriends);
                }
                else
                {
                    ClassHelper.MessageAlert(ClassHelper.MainWindow.GetType(), 1, ClassHelper.FindResource<string>("PhoneNumberNotCorrect"));
                }
            }
        }

        private void BtnFriendAddSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFriendAdd.Text))
            {
                ThreadPool.QueueUserWorkItem(FriendAddSend, txtFriendAdd.Text);
            }
            else
            {
                ClassHelper.MessageAlert(ClassHelper.MainWindow.GetType(), 1, ClassHelper.FindResource<string>("VerificationInformationCannotEmpty"));
            }
        }

        private void ItemsControl_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void SignalRClientHelper_FriendOnlineSignalR(string friendID, bool online)
        {
            if (mainData.Friends != null)
            {
                foreach (FriendSortModel item in mainData.Friends)
                {
                    if (item.FriendBriefs.Find(f => f.UserID == friendID) is FriendBriefModel friend)
                    {
                        friend.OnLine = online;
                    }
                }
            }
        }

        private void SignalRClientHelper_NewFriendSignalR(string friendID)
        {
            if (UserManagerService.GetNewFriendList(out List<NewFriendBriefModel> newFriendBrief))
            {
                mainData.NewFriends = newFriendBrief;
            }
        }

        private void SignalRClientHelper_FriendChangedSignalR(string sort, string friendID, bool state)
        {
            if (UserManagerService.GetFriendList(out List<FriendSortModel> friendSorts))
            {
                mainData.Friends = friendSorts;
            }
            if (UserManagerService.GetNewFriendList(out List<NewFriendBriefModel> newFriendBrief))
            {
                mainData.NewFriends = newFriendBrief;
            }
        }

        private void SignalRClientHelper_RemarkInfoChangedSignalR(string friendID)
        {
            if (mainData.Friends != null)
            {
                if (UserManagerService.GetFriendInfo(out FriendDetailsModel friendDetails, friendID: friendID))
                {
                    foreach (FriendSortModel item in mainData.Friends)
                    {
                        if (item.FriendBriefs.Find(f => f.UserID == friendID) is FriendBriefModel friend)
                        {
                            friend.RemarkName = friendDetails.RemarkName;
                        }
                    }
                }
            }
        }

        #region 执行事件
        private void Load(object data)
        {
            if (mainData.Friends == null)
            {
                if (UserManagerService.GetFriendList(out List<FriendSortModel> friendSorts))
                {
                    mainData.Friends = friendSorts;
                }
            }
            if (mainData.NewFriends == null)
            {
                if (UserManagerService.GetNewFriendList(out List<NewFriendBriefModel> newFriendBrief))
                {
                    mainData.NewFriends = newFriendBrief;
                }
            }
        }
        private void TxbAddFriends_PointerUp()
        {
            brdAddFriends.Tag = "1";
            brdAddFriends.Visibility = Visibility.Visible;
        }
        private async void TxbPopupClose_PointerUp()
        {
            brdAddFriends.Tag = "0";
            await Task.Delay(350);
            brdAddFriends.Visibility = Visibility.Collapsed;
            newFriendData.InitializeVariable();
            stfAddFriends.BeginAnimation(ScaleTransform.ScaleYProperty, null);
        }
        private void FindFriends(object data)
        {
            Dispatcher.Invoke(delegate
            {
                btnFind.IsEnabled = false;
            });

            if (UserManagerService.GetFriendInfo(out FriendDetailsModel friendDetails, phoneNumber: newFriendData.PhoneNumber))
            {
                newFriendData.UserID = friendDetails.UserID;
                newFriendData.NickName = friendDetails.NickName;
                newFriendData.HeadPortrait = friendDetails.HeadPortrait;
                newFriendData.PhoneNumber = friendDetails.PhoneNumber;
                newFriendData.Personalized = friendDetails.Personalized;
                newFriendData.Friend = friendDetails.Friend;
                Dispatcher.Invoke(delegate
                {
                    if (stfAddFriends.ScaleY == 0)
                    {
                        DoubleAnimation doubleAnimation = new()
                        {
                            To = 1,
                            Duration = new TimeSpan(0, 0, 0, 0, 300),
                            EasingFunction = new CircleEase
                            {
                                EasingMode = EasingMode.EaseOut
                            }
                        };
                        stfAddFriends.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
                    }
                });
            }

            Dispatcher.Invoke(delegate
            {
                btnFind.IsEnabled = true;
            });
        }
        private void FriendAddSend(object data)
        {
            Dispatcher.Invoke(delegate
            {
                btnFriendAddSend.IsEnabled = false;
            });

            if (UserManagerService.FriendRegistration(newFriendData.UserID, data.ToString()))
            {
                FindFriends(null);
                ClassHelper.MessageAlert(ClassHelper.MainWindow.GetType(), 0, ClassHelper.FindResource<string>("SendSuccessfully"));
                Dispatcher.Invoke(delegate
                {
                    popFriendAdd.IsOpen = false;
                });
            }

            Dispatcher.Invoke(delegate
            {
                btnFriendAddSend.IsEnabled = true;
            });
        }
        #endregion
    }
}
