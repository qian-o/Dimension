using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.Chat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ChatItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatItem : UserControl
    {
        private ChatColumnInfoModel chatColumn;
        private static Border borderSelect;
        private readonly ItemsControl itemsControl;

        public ChatItem()
        {
            InitializeComponent();

            itemsControl = new ItemsControl
            {
                Style = ClassHelper.FindResource<Style>("ItemsControlVirtualization"),
                ItemTemplateSelector = ClassHelper.FindResource<DataTemplateSelector>("ChatTemplateSelector")
            };
            itemsControl.Loaded += ItemsControl_Loaded;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.NewMessageSignalR += SignalRClientHelper_NewMessageSignalR;
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.NewMessageSignalR -= SignalRClientHelper_NewMessageSignalR;
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (itemsControl.Tag == null)
            {
                if (VisualTreeHelper.GetChild(itemsControl, 0) is ScrollViewer scroll)
                {
                    scroll.ScrollToEnd();
                }
                itemsControl.Tag = true;
            }
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ChatColumnInfoModel chatColumnInfo)
            {
                imgHead.DataContext = chatColumnInfo.HeadPortrait;
                txbNickName.Text = chatColumnInfo.NickName;
                txbRemarkName.Text = chatColumnInfo.RemarkName;
                chatColumn = chatColumnInfo;
                chatColumn.ChatContent = new ObservableCollection<ChatMessagesModel>();
                chatColumn.Items = itemsControl;

                itemsControl.ItemsSource = chatColumn.ChatContent;
                ThreadPool.QueueUserWorkItem(Load, chatColumnInfo.ChatID);
            }
        }

        #region 选择好友进行聊天(鼠标,触控)
        private void BrdChat_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdChat_PointerUp(sender);
            }
        }
        private void BrdChat_TouchUp(object sender, TouchEventArgs e)
        {
            BrdChat_PointerUp(sender);
        }
        #endregion

        private void SignalRClientHelper_NewMessageSignalR(string chatID)
        {
            if (chatColumn.ChatID == chatID)
            {
                lock (chatColumn.ChatContent)
                {
                    if (ChatService.GetChattingRecords(chatID, out List<ChatMessagesModel> chatMessages))
                    {
                        foreach (ChatMessagesModel item in chatMessages)
                        {
                            if (chatColumn.ChatContent.FirstOrDefault(c => c.ID == item.ID) == null)
                            {
                                Dispatcher.Invoke(delegate
                                {
                                    if (itemsControl.IsLoaded)
                                    {
                                        if (VisualTreeHelper.GetChild(itemsControl, 0) is ScrollViewer scroll)
                                        {
                                            if (scroll.VerticalOffset > scroll.ScrollableHeight - 50 || item.SenderID == ClassHelper.UserID)
                                            {
                                                scroll.ScrollToEnd();
                                            }
                                        }
                                    }

                                    chatColumn.ChatContent.Add(item);
                                });
                            }
                        }
                    }
                }

            }
        }

        #region 执行事件
        private void Load(object data)
        {
            if (chatColumn.ChatContent.Count == 0)
            {
                if (ChatService.GetChattingRecords(data.ToString(), out List<ChatMessagesModel> chatMessages))
                {
                    Dispatcher.Invoke(delegate
                    {
                        foreach (ChatMessagesModel item in chatMessages)
                        {
                            chatColumn.ChatContent.Add(item);
                        }

                    });
                }
            }
        }
        private void BrdChat_PointerUp(object sender)
        {
            Border border = sender as Border;
            border.IsEnabled = false;
            if (borderSelect != null)
            {
                borderSelect.IsEnabled = true;
            }
            borderSelect = border;
            ClassHelper.TransferringData(typeof(ChatMain), chatColumn);
        }
        #endregion
    }
}
