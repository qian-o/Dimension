using DimensionClient.Common;
using DimensionClient.Library.Converters;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.Chat;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        public MasterChat MasterChat { get; private set; } = new MasterChat();

        public ChatItem()
        {
            InitializeComponent();

            imgHead.SetBinding(DataContextProperty, "HeadPortrait");
            txbNickName.SetBinding(Run.TextProperty, "NickName");
            txbRemarkName.SetBinding(Run.TextProperty, "RemarkName");
            brdBadge.SetBinding(VisibilityProperty, new Binding { Path = new PropertyPath("Unread"), Converter = ClassHelper.FindResource<BoolVisibilityConvert>("BoolVisibilityConvert") });
            txbBadgeNumber.SetBinding(TextBlock.TextProperty, "Unread");

            MasterChat.itcMasterChat.SetBinding(ItemsControl.ItemsSourceProperty, "ChatContent");
            MasterChat.brdUnread.IsVisibleChanged += BrdUnread_IsVisibleChanged;
            MasterChat.Loaded += MasterChat_Loaded;
            MasterChat.Unloaded += MasterChat_Unloaded;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ClassHelper.ChatFriendID))
            {
                ClassHelper.ChatFriendID = chatColumn.FriendID;
            }
            if (chatColumn.FriendID == ClassHelper.ChatFriendID)
            {
                if (brdChat != borderSelect)
                {
                    brdChat.IsEnabled = false;
                    if (borderSelect != null)
                    {
                        borderSelect.IsEnabled = true;
                    }
                    borderSelect = brdChat;
                    ClassHelper.TransferringData(typeof(ChatMain), this);
                }
            }
            SignalRClientHelper.NewMessageSignalR += SignalRClientHelper_NewMessageSignalR;
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            SignalRClientHelper.NewMessageSignalR -= SignalRClientHelper_NewMessageSignalR;
        }

        private void MasterChat_Loaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChild(MasterChat.itcMasterChat, 0) is ScrollViewer scroll)
            {
                scroll.ScrollChanged += Scroll_ScrollChanged;
                if (MasterChat.itcMasterChat.Tag == null || chatColumn.Unread > 0)
                {
                    if (MasterChat.brdUnread.Visibility == Visibility.Collapsed && chatColumn.Unread > 0)
                    {
                        ThreadPool.QueueUserWorkItem(ReadMessage);
                    }
                    scroll.ScrollToEnd();
                    MasterChat.itcMasterChat.Tag = true;
                }
            }

        }

        private void MasterChat_Unloaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChild(MasterChat.itcMasterChat, 0) is ScrollViewer scroll)
            {
                scroll.ScrollChanged -= Scroll_ScrollChanged;
            }
        }

        private void Scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (scroll.VerticalOffset > scroll.ScrollableHeight - 50)
            {
                MasterChat.brdUnread.Visibility = Visibility.Collapsed;
            }
        }

        private void BrdUnread_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (chatColumn.Unread > 0 && MasterChat.brdUnread.Visibility == Visibility.Collapsed)
            {
                ThreadPool.QueueUserWorkItem(ReadMessage);
            }
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ChatColumnInfoModel chatColumnInfo)
            {
                chatColumn = chatColumnInfo;
                chatColumn.ChatContent = new ObservableCollection<ChatMessagesModel>();
                chatColumn.ChatContent.CollectionChanged += ChatContent_CollectionChanged;

                MasterChat.DataContext = chatColumn;

                ThreadPool.QueueUserWorkItem(Load, chatColumnInfo.ChatID);
            }
        }

        private void ChatContent_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (chatColumn.ChatContent.LastOrDefault() is ChatMessagesModel chatMessages)
            {
                switch (chatMessages.MessageType)
                {
                    case ClassHelper.MessageType.Text:
                        txbLastMessage.Text = chatMessages.MessageContent.Replace("\r\n", string.Empty);
                        break;
                    case ClassHelper.MessageType.Voice:
                        txbLastMessage.Text = $"[{ClassHelper.FindResource<string>("VoiceMessage")}]";
                        break;
                    case ClassHelper.MessageType.File:
                        {
                            FileModel fileModel = JsonConvert.DeserializeObject<FileModel>(chatMessages.MessageContent);
                            txbLastMessage.Text = fileModel.FileType == ClassHelper.FileType.Image
                                ? $"[{ClassHelper.FindResource<string>("ImageMessage")}]"
                                : $"[{ClassHelper.FindResource<string>("AccessoryMessage")}]";
                        }
                        break;
                    case ClassHelper.MessageType.VoiceTalk:
                        txbLastMessage.Text = $"[{ClassHelper.FindResource<string>("VoiceTalk")}]";
                        break;
                    case ClassHelper.MessageType.VideoTalk:
                        txbLastMessage.Text = $"[{ClassHelper.FindResource<string>("VideoTalk")}]";
                        break;
                    default:
                        break;
                }
                txbLastTime.Text = chatMessages.CreateTime.ToString("t", ClassHelper.cultureInfo);
            }
        }

        #region 选择好友进行聊天(鼠标,触控)
        private void BrdChat_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                BrdChat_PointerUp();
            }
        }
        private void BrdChat_TouchUp(object sender, TouchEventArgs e)
        {
            BrdChat_PointerUp();
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
                                item.LoadFirst = true;
                                Dispatcher.Invoke(delegate
                                {
                                    if (MasterChat.IsLoaded)
                                    {
                                        if (VisualTreeHelper.GetChild(MasterChat.itcMasterChat, 0) is ScrollViewer scroll)
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
                        int unread = chatMessages.Where(item => item.ReceiverID == ClassHelper.UserID && !item.IsRead && !item.IsWithdraw).Count();
                        Dispatcher.Invoke(delegate
                        {
                            if (MasterChat.IsLoaded && unread > 0)
                            {
                                if (VisualTreeHelper.GetChild(MasterChat.itcMasterChat, 0) is ScrollViewer scroll)
                                {
                                    if (scroll.VerticalOffset > scroll.ScrollableHeight - 50)
                                    {
                                        ThreadPool.QueueUserWorkItem(ReadMessage);
                                    }
                                    else
                                    {
                                        chatColumn.Unread = unread;
                                        MasterChat.brdUnread.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                            else
                            {
                                chatColumn.Unread = unread;
                            }
                        });
                    }
                }

            }
        }

        #region 执行事件
        private void Load(object data)
        {
            lock (chatColumn.ChatContent)
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
                        chatColumn.Unread = chatMessages.Where(item => item.ReceiverID == ClassHelper.UserID && !item.IsRead && !item.IsWithdraw).Count();
                        Dispatcher.Invoke(delegate
                        {
                            if (MasterChat.IsLoaded)
                            {
                                if (VisualTreeHelper.GetChild(MasterChat.itcMasterChat, 0) is ScrollViewer scroll)
                                {
                                    scroll.ScrollToEnd();
                                    if (chatColumn.Unread > 0)
                                    {
                                        ThreadPool.QueueUserWorkItem(ReadMessage);
                                    }
                                }
                            }
                        });
                    }
                }
            }
        }
        private void BrdChat_PointerUp()
        {
            brdChat.IsEnabled = false;
            if (borderSelect != null)
            {
                borderSelect.IsEnabled = true;
            }
            borderSelect = brdChat;
            ClassHelper.ChatFriendID = chatColumn.FriendID;
            ClassHelper.TransferringData(typeof(ChatMain), this);
        }
        private void ReadMessage(object data)
        {
            lock (chatColumn.ChatContent)
            {
                if (chatColumn.ChatContent.LastOrDefault(item => item.SenderID != ClassHelper.UserID && !item.IsRead) is ChatMessagesModel chatMessages)
                {
                    if (ChatService.ReadMessage(chatColumn.ChatID, chatMessages.ID))
                    {
                        foreach (ChatMessagesModel item in chatColumn.ChatContent.Where(item => item.ReceiverID == ClassHelper.UserID && !item.IsRead && !item.IsWithdraw))
                        {
                            item.IsRead = true;
                        }

                        chatColumn.Unread = 0;
                    }
                }
            }
        }
        #endregion
    }
}
