using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// FriendItem.xaml 的交互逻辑
    /// </summary>
    public partial class FriendItem : UserControl
    {
        private static Border borderSelect;

        public FriendItem()
        {
            InitializeComponent();
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is FriendSortModel friend)
            {
                txbSort.Text = friend.Sort;
                itcFriendBriefs.ItemsSource = friend.FriendBriefs;
            }
        }

        private void BrdDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            if ((border.Tag as FriendBriefModel).UserID == ClassHelper.FriendID)
            {
                border.IsEnabled = false;
                borderSelect = border;
            }
        }

        #region 获取好友详细信息(鼠标,触控)
        private void BrdDetail_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdDetail_PointerUp(sender);
            }
        }
        private void BrdDetail_TouchUp(object sender, TouchEventArgs e)
        {
            BrdDetail_PointerUp(sender);
        }
        #endregion

        #region 执行事件
        private static void BrdDetail_PointerUp(object sender)
        {
            Border border = sender as Border;
            border.IsEnabled = false;
            if (borderSelect != null)
            {
                borderSelect.IsEnabled = true;
            }
            borderSelect = border;
            ClassHelper.TransferringData(typeof(FriendInfoCard), (border.Tag as FriendBriefModel).UserID);
        }
        #endregion
    }
}
