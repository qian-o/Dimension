using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.Chat;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

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
            ThreadPool.QueueUserWorkItem(SendMessage);
        }

        private void ClassHelper_DataPassingChanged(object data)
        {
            ChatColumnInfoModel chatColumnInfo = data as ChatColumnInfoModel;
            chatMainData.ChatID = chatColumnInfo.ChatID;
            chatMainData.FriendNickName = string.IsNullOrEmpty(chatColumnInfo.RemarkName) ? chatColumnInfo.NickName : chatColumnInfo.RemarkName;
            brdChat.Child = chatColumnInfo.Items;
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
