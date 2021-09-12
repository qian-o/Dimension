using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.Chat;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ChatMain.xaml 的交互逻辑
    /// </summary>
    public partial class ChatMain : UserControl
    {
        private readonly ChatMainViewModel chatMainData;
        public ChatMain()
        {
            InitializeComponent();

            chatMainData = DataContext as ChatMainViewModel;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            ClassHelper.DataPassingChanged += ClassHelper_DataPassingChanged;
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            ClassHelper.DataPassingChanged -= ClassHelper_DataPassingChanged;
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(chatMainData.MessageText))
            {
                ThreadPool.QueueUserWorkItem(SendMessage);
            }
        }

        private void ClassHelper_DataPassingChanged(object data)
        {
            ChatItem chatItem = data as ChatItem;
            ChatColumnInfoModel chatColumnInfo = chatItem.DataContext as ChatColumnInfoModel;
            chatMainData.ChatID = chatColumnInfo.ChatID;
            txbFriendNickName.SetBinding(TextBlock.TextProperty, new Binding { Source = chatColumnInfo, Path = new PropertyPath(string.IsNullOrEmpty(chatColumnInfo.RemarkName) ? nameof(chatColumnInfo.NickName) : nameof(chatColumnInfo.RemarkName)) });
            brdChat.Child = chatItem.MasterChat;
        }

        #region 执行事件
        private void SendMessage(object data)
        {
            Dispatcher.Invoke(delegate
            {
                btnSend.IsEnabled = false;
            });

            if (ChatService.SendMessage(chatMainData.ChatID, ClassHelper.MessageType.Text, chatMainData.MessageText))
            {
                chatMainData.MessageText = string.Empty;
            }

            Dispatcher.Invoke(delegate
            {
                btnSend.IsEnabled = true;
            });
        }
        #endregion
    }
}
