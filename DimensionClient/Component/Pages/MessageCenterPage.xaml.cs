using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.Chat;
using DimensionClient.Service.UserManager;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DimensionClient.Component.Pages
{
    /// <summary>
    /// MessageCenterPage.xaml 的交互逻辑
    /// </summary>
    public partial class MessageCenterPage : Page
    {
        private readonly MessageCenterViewModel mainData;

        public MessageCenterPage()
        {
            InitializeComponent();

            mainData = DataContext as MessageCenterViewModel;
            SignalRClientHelper.RemarkInfoChangedSignalR += SignalRClientHelper_RemarkInfoChangedSignalR;
            SignalRClientHelper.ChatColumnChangedSignalR += SignalRClientHelper_ChatColumnChangedSignalR;
        }

        private void MessageCenterMain_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(Load);
        }

        private void MessageCenterMain_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void SignalRClientHelper_RemarkInfoChangedSignalR(string friendID)
        {
            if (UserManagerService.GetFriendInfo(out FriendDetailsModel friendDetails, friendID: friendID))
            {
                foreach (ChatColumnInfoModel item in mainData.ChatColumnInfos)
                {
                    if (item.FriendID == friendID)
                    {
                        item.RemarkName = friendDetails.RemarkName;
                    }
                }
            }
        }

        private void SignalRClientHelper_ChatColumnChangedSignalR(string friendID)
        {
            lock (mainData.ChatColumnInfos)
            {
                if (ChatService.GetChatColumnInfo(out List<ChatColumnInfoModel> chatColumnInfos))
                {
                    foreach (ChatColumnInfoModel item in chatColumnInfos)
                    {
                        if (mainData.ChatColumnInfos.FirstOrDefault(c => c.ChatID == item.ChatID) == null)
                        {
                            Dispatcher.Invoke(delegate
                            {
                                mainData.ChatColumnInfos.Add(item);
                            });
                        }
                    }
                }
            }
        }

        private void ItemsControl_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        #region 执行事件
        private void Load(object data)
        {
            if (mainData.ChatColumnInfos.Count == 0)
            {
                if (ChatService.GetChatColumnInfo(out List<ChatColumnInfoModel> chatColumnInfos))
                {
                    Dispatcher.Invoke(delegate
                    {
                        foreach (ChatColumnInfoModel item in chatColumnInfos)
                        {
                            mainData.ChatColumnInfos.Add(item);
                        }
                    });
                }
            }
        }
        #endregion
    }
}
