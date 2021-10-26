using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.Chat;
using DimensionClient.Service.UserManager;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// FriendInfoCard.xaml 的交互逻辑
    /// </summary>
    public partial class FriendInfoCard : UserControl
    {
        private readonly FriendDetailsModel friendData;

        public FriendInfoCard()
        {
            InitializeComponent();

            friendData = DataContext as FriendDetailsModel;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            ClassHelper.DataPassingChanged += ClassHelper_DataPassingChanged;
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            ClassHelper.DataPassingChanged -= ClassHelper_DataPassingChanged;
        }

        #region 修改好友备注(鼠标,触控)
        private void TxbRemark_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbRemark_PointerUp(sender);
            }
        }
        private void TxbRemark_TouchUp(object sender, TouchEventArgs e)
        {
            TxbRemark_PointerUp(sender);
        }
        #endregion

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(SendMessage);
        }

        private void TxtRemark_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = sender as TextBox;
            if (text.Tag.ToString() == "RemarkName")
            {
                txbRemarkName.Visibility = Visibility.Visible;
                txtRemarkName.Visibility = Visibility.Collapsed;
            }
            else if (text.Tag.ToString() == "RemarkInformation")
            {
                txbRemarkInformation.Visibility = Visibility.Visible;
                txtRemarkInformation.Visibility = Visibility.Collapsed;
            }
        }

        private void ClassHelper_DataPassingChanged(ClassHelper.DataPassingType dataType, object data)
        {
            if (dataType == ClassHelper.DataPassingType.SelectFriend)
            {
                ClassHelper.ContactPersonFriendID = data.ToString();
                ThreadPool.QueueUserWorkItem(GetFriendDetails, ClassHelper.ContactPersonFriendID);
                Dispatcher.Invoke(delegate
                {
                    Visibility = Visibility.Visible;
                });
            }
        }

        #region 执行事件
        private void GetFriendDetails(object data)
        {
            Dispatcher.Invoke(delegate
            {
                txbRemarkNameAction.Text = "\xe78c";
                txbRemarkInformationAction.Text = "\xe78c";
            });
            if (UserManagerService.GetFriendInfo(out FriendDetailsModel friendDetails, friendID: ClassHelper.ContactPersonFriendID))
            {
                if (ClassHelper.ContactPersonFriendID == friendDetails.UserID)
                {
                    friendData.UserID = friendDetails.UserID;
                    friendData.NickName = friendDetails.NickName;
                    friendData.RemarkName = friendDetails.RemarkName;
                    friendData.HeadPortrait = friendDetails.HeadPortrait;
                    friendData.PhoneNumber = friendDetails.PhoneNumber;
                    friendData.Email = friendDetails.Email;
                    friendData.Gender = friendDetails.Gender;
                    friendData.Birthday = friendDetails.Birthday;
                    friendData.Location = friendDetails.Location;
                    friendData.Profession = friendDetails.Profession;
                    friendData.Personalized = friendDetails.Personalized;
                    friendData.RemarkInformation = friendDetails.RemarkInformation;
                    friendData.OnLine = friendDetails.OnLine;
                    friendData.Friend = friendDetails.Friend;
                }
            }
        }
        private void TxbRemark_PointerUp(object sender)
        {
            TextBlock text = sender as TextBlock;
            if (text.Tag.ToString() == "RemarkName")
            {
                if (txtRemarkName.Visibility == Visibility.Collapsed)
                {
                    txbRemarkName.Visibility = Visibility.Collapsed;
                    txtRemarkName.Visibility = Visibility.Visible;
                    txtRemarkName.Text = friendData.RemarkName;
                    txtRemarkName.Focus();
                    txtRemarkName.CaretIndex = txtRemarkName.Text.Length;
                }
                else
                {
                    txbRemarkName.Visibility = Visibility.Visible;
                    txtRemarkName.Visibility = Visibility.Collapsed;
                    if (txtRemarkName.Text != friendData.RemarkName)
                    {
                        friendData.RemarkName = txtRemarkName.Text;
                        string[] datas = { friendData.RemarkName, null };
                        ThreadPool.QueueUserWorkItem(UpdateRemarkInfo, datas);
                    }
                }
            }
            else if (text.Tag.ToString() == "RemarkInformation")
            {
                if (txtRemarkInformation.Visibility == Visibility.Collapsed)
                {
                    txbRemarkInformation.Visibility = Visibility.Collapsed;
                    txtRemarkInformation.Visibility = Visibility.Visible;
                    txtRemarkInformation.Text = friendData.RemarkInformation;
                    txtRemarkInformation.Focus();
                    txtRemarkInformation.CaretIndex = txtRemarkInformation.Text.Length;
                }
                else
                {
                    txbRemarkInformation.Visibility = Visibility.Visible;
                    txtRemarkInformation.Visibility = Visibility.Collapsed;
                    if (txtRemarkInformation.Text != friendData.RemarkInformation)
                    {
                        friendData.RemarkInformation = txtRemarkInformation.Text;
                        string[] datas = { null, friendData.RemarkInformation };
                        ThreadPool.QueueUserWorkItem(UpdateRemarkInfo, datas);
                    }
                }
            }
        }
        private void UpdateRemarkInfo(object data)
        {
            string[] datas = (string[])data;
            Dispatcher.Invoke(delegate
            {
                if (datas[0] != null)
                {
                    txbRemarkNameAction.Text = "\xe6cf";
                }
                if (datas[1] != null)
                {
                    txbRemarkInformationAction.Text = "\xe6cf";
                }
            });

            if (UserManagerService.UpdateRemarkInfo(friendData.UserID, datas[0], datas[1]))
            {
                if (ClassHelper.ContactPersonFriendID == friendData.UserID)
                {
                    if (UserManagerService.GetFriendInfo(out FriendDetailsModel friendDetails, friendID: ClassHelper.ContactPersonFriendID))
                    {
                        if (ClassHelper.ContactPersonFriendID == friendDetails.UserID)
                        {
                            friendData.RemarkName = friendDetails.RemarkName;
                            friendData.RemarkInformation = friendDetails.RemarkInformation;
                        }
                    }
                }
            }

            Dispatcher.Invoke(delegate
            {
                if (datas[0] != null)
                {
                    txbRemarkNameAction.Text = "\xe78c";
                }
                if (datas[1] != null)
                {
                    txbRemarkInformationAction.Text = "\xe78c";
                }
            });
        }
        private void SendMessage(object data)
        {
            if (ChatService.AddChat(friendData.UserID))
            {
                ClassHelper.ChatFriendID = friendData.UserID;
                ClassHelper.SwitchRoute(ClassHelper.PageType.MessageCenterPage);
            };
        }
        #endregion
    }
}