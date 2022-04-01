using Dimension.Domain;
using DimensionClient.Common;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.UserManager;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// NewFriendItem.xaml 的交互逻辑
    /// </summary>
    public partial class NewFriendItem : UserControl
    {
        private NewFriendBriefModel friendBriefModel;

        public NewFriendItem()
        {
            InitializeComponent();
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is NewFriendBriefModel newFriendBrief)
            {
                imgHead.DataContext = newFriendBrief.HeadPortrait;
                txbNickName.Text = newFriendBrief.NickName;
                txbVerifyInfo.Text = newFriendBrief.VerifyInfo;
                if (newFriendBrief.FriendType == NewFriendType.Verify && newFriendBrief.Passed == null)
                {
                    brdAgree.Visibility = Visibility.Visible;
                    txbState.Visibility = Visibility.Collapsed;
                }
                else
                {
                    brdAgree.Visibility = Visibility.Collapsed;
                    txbState.Visibility = Visibility.Visible;
                    txbState.Text = newFriendBrief.Passed switch
                    {
                        true => ClassHelper.FindResource<string>("Agreed"),
                        false => ClassHelper.FindResource<string>("Refused"),
                        _ => ClassHelper.FindResource<string>("ToBeConfirm"),
                    };
                }
                friendBriefModel = newFriendBrief;
            }
        }

        #region 同意添加(鼠标,触控)
        private void BrdAgree_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdAgree_PointerUp();
            }
        }
        private void BrdAgree_TouchUp(object sender, TouchEventArgs e)
        {
            BrdAgree_PointerUp();
        }
        #endregion

        #region 执行事件
        private void BrdAgree_PointerUp()
        {
            MessageBoxButtonModel rightButton = new()
            {
                Action = new System.Action(() =>
                {
                    UserManagerService.FriendValidation(friendBriefModel.UserID, true);
                })
            };
            ClassHelper.AlertMessageBox(ClassHelper.MainWindow, ClassHelper.MessageBoxType.Select, ClassHelper.FindResource<string>("AddFriendReminder").Replace("{0}", friendBriefModel.NickName), rightButton: rightButton);
        }
        #endregion
    }
}
