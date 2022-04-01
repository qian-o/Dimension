using Dimension.Domain;
using DimensionClient.Common;
using DimensionClient.Library.Converters;
using DimensionClient.Library.CustomControls;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.Chat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using static DimensionClient.Common.ClassHelper;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ChatItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatItem : UserControl
    {
        private readonly ScrollViewer scroll;
        private ChatColumnInfoModel chatColumn;
        private static Border borderSelect;
        public MasterChat MasterChat { get; private set; } = new MasterChat();

        public ChatItem()
        {
            InitializeComponent();

            imgHead.SetBinding(DataContextProperty, "HeadPortrait");
            txbNickName.SetBinding(Run.TextProperty, "NickName");
            txbRemarkName.SetBinding(Run.TextProperty, "RemarkName");
            brdBadge.SetBinding(VisibilityProperty, new Binding { Path = new PropertyPath("Unread"), Converter = FindResource<BoolVisibilityConvert>("BoolVisibilityConvert") });
            txbBadgeNumber.SetBinding(TextBlock.TextProperty, "Unread");

            MasterChat.itcMasterChat.SetBinding(ItemsControl.ItemsSourceProperty, "ChatContent");
            MasterChat.brdUnread.IsVisibleChanged += BrdUnread_IsVisibleChanged;
            MasterChat.Loaded += MasterChat_Loaded;

            MasterChat.itcMasterChat.ApplyTemplate();
            scroll = MasterChat.itcMasterChat.Template.FindName("sclItems", MasterChat.itcMasterChat) as ScrollViewer;
            scroll.ScrollChanged += Scroll_ScrollChanged;

            SignalRClientHelper.NewMessageSignalR += SignalRClientHelper_NewMessageSignalR;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ChatFriendID))
            {
                ChatFriendID = chatColumn.FriendID;
            }
            if (chatColumn.FriendID == ChatFriendID)
            {
                if (brdChat != borderSelect)
                {
                    brdChat.IsEnabled = false;
                    if (borderSelect != null)
                    {
                        borderSelect.IsEnabled = true;
                    }
                    borderSelect = brdChat;
                    TransferringData(typeof(ChatMain), DataPassingType.SelectMessage, this);
                }
            }
        }

        private void MasterChat_Loaded(object sender, RoutedEventArgs e)
        {
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
                    case MessageType.Text:
                        txbLastMessage.Text = chatMessages.MessageContent.Replace("\r\n", string.Empty);
                        break;
                    case MessageType.RichText:
                        txbLastMessage.Text = $"[{FindResource<string>("RichMessage")}]";
                        break;
                    case MessageType.Voice:
                        txbLastMessage.Text = $"[{FindResource<string>("VoiceMessage")}]";
                        break;
                    case MessageType.File:
                        {
                            FileModel fileModel = JsonConvert.DeserializeObject<FileModel>(chatMessages.MessageContent);
                            txbLastMessage.Text = fileModel.FileType == FileType.Image
                                ? $"[{FindResource<string>("ImageMessage")}]"
                                : $"[{FindResource<string>("AccessoryMessage")}]";
                        }
                        break;
                    case MessageType.VoiceTalk:
                        txbLastMessage.Text = $"[{FindResource<string>("VoiceTalk")}]";
                        break;
                    case MessageType.VideoTalk:
                        txbLastMessage.Text = $"[{FindResource<string>("VideoTalk")}]";
                        break;
                    default:
                        break;
                }
                txbLastTime.Text = chatMessages.CreateTime.ToString("t");
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
                                    if (scroll.VerticalOffset > scroll.ScrollableHeight - 50 || item.SenderID == UserID)
                                    {
                                        scroll.ScrollToEnd();
                                    }
                                    chatColumn.ChatContent.Add(item);
                                });
                            }
                        }
                        int unread = chatMessages.Where(item => item.ReceiverID == UserID && !item.IsRead && !item.IsWithdraw).Count();
                        Dispatcher.Invoke(delegate
                        {
                            if (MasterChat.IsLoaded && unread > 0)
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
                        chatColumn.Unread = chatMessages.Where(item => item.ReceiverID == UserID && !item.IsRead && !item.IsWithdraw).Count();
                        Dispatcher.Invoke(delegate
                        {
                            if (MasterChat.IsLoaded)
                            {
                                scroll.ScrollToEnd();
                                if (chatColumn.Unread > 0)
                                {
                                    ThreadPool.QueueUserWorkItem(ReadMessage);
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
            ChatFriendID = chatColumn.FriendID;
            TransferringData(typeof(ChatMain), DataPassingType.SelectMessage, this);
        }
        public void Send()
        {
            ThreadPool.QueueUserWorkItem(SendMessage);
        }
        private void ReadMessage(object data)
        {
            lock (chatColumn.ChatContent)
            {
                if (chatColumn.ChatContent.LastOrDefault(item => item.SenderID != UserID && !item.IsRead) is ChatMessagesModel chatMessages)
                {
                    if (ChatService.ReadMessage(chatColumn.ChatID, chatMessages.ID))
                    {
                        foreach (ChatMessagesModel item in chatColumn.ChatContent.Where(item => item.ReceiverID == UserID && !item.IsRead && !item.IsWithdraw))
                        {
                            item.IsRead = true;
                        }

                        chatColumn.Unread = 0;
                    }
                }
            }
        }
        private async void SendMessage(object data)
        {
            chatColumn.IsUsable = false;

            List<Task> uploading = new();
            RichMessageModel richMessage = new();
            Dispatcher.Invoke(delegate
            {
                foreach (Block item in chatColumn.Flow.Blocks)
                {
                    if (item is Paragraph paragraph)
                    {
                        if (item != chatColumn.Flow.Blocks.FirstBlock)
                        {
                            richMessage.RichMessageContents.Add(new RichMessageContentModel
                            {
                                MessageType = RichMessageType.Text,
                                Content = Environment.NewLine,
                                FileAttribute = null
                            });
                        }
                        foreach (Inline coll in paragraph.Inlines)
                        {
                            if (coll is Run run)
                            {
                                richMessage.RichMessageContents.Add(new RichMessageContentModel
                                {
                                    MessageType = RichMessageType.Text,
                                    Content = run.Text,
                                    FileAttribute = null
                                });
                            }
                            else if (coll is InlineUIContainer con)
                            {
                                if (con.Child is ChatImage chatImage)
                                {
                                    RichMessageContentModel richMessageContent = new()
                                    {
                                        MessageType = RichMessageType.Image
                                    };
                                    richMessage.RichMessageContents.Add(richMessageContent);
                                    Task task = new((e) =>
                                    {
                                        RichMessageContentModel contentModel = e as RichMessageContentModel;
                                        MultipartFormDataContent dataContent = new();
                                        double fileSize = 0;
                                        double fileWidth = 0;
                                        double fileHeight = 0;
                                        Dispatcher.Invoke(delegate
                                        {
                                            using MemoryStream memoryStream = new();
                                            string extend = new FileInfo(chatImage.PathUri.LocalPath).Extension.ToLower();
                                            File.OpenRead(chatImage.PathUri.LocalPath).CopyTo(memoryStream);
                                            BitmapSource bitmapSource = new BitmapImage(chatImage.PathUri);
                                            dataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "file", $"{GetRandomString(10)}{extend}");

                                            fileSize = (double)memoryStream.Length / 1000 / 1000;
                                            fileWidth = bitmapSource.Width;
                                            fileHeight = bitmapSource.Height;

                                            memoryStream.Close();
                                        });
                                        if (ServerUpload($"{servicePath}/api/Attachment/UploadAttachment", dataContent, out string fileName))
                                        {
                                            contentModel.Content = fileName;
                                            contentModel.FileAttribute = new FileModel()
                                            {
                                                FileType = FileType.Image,
                                                FileName = fileName,
                                                FileMByte = fileSize,
                                                FileWidth = fileWidth,
                                                FileHeight = fileHeight
                                            };
                                            Dispatcher.Invoke(delegate
                                            {
                                                chatImage.PathUri = new Uri(fileName, UriKind.Relative);
                                                chatImage.FileWidth = fileWidth;
                                                chatImage.FileHeight = fileHeight;
                                                chatImage.IsLoadRelative = true;
                                                chatImage.UnLoad();
                                            });
                                        }
                                    }, richMessageContent);
                                    task.Start();
                                    uploading.Add(task);
                                }
                            }
                        }
                    }

                }
            });
            List<RichMessageContentModel> richesText = richMessage.Filter(RichMessageType.Text);
            List<RichMessageContentModel> richesImage = richMessage.Filter(RichMessageType.Image);
            if (richesImage.Count > 0)
            {
                await Task.WhenAll(uploading);
                if (richesText.Count == 0)
                {
                    foreach (RichMessageContentModel item in richesImage)
                    {
                        ChatService.SendMessage(chatColumn.ChatID, MessageType.File, JObject.FromObject(item.FileAttribute).ToString());
                    }
                }
                else
                {
                    Dispatcher.Invoke(delegate
                    {
                        richMessage.SerializedMessage = XamlWriter.Save(chatColumn.Flow);
                    });
                    ChatService.SendMessage(chatColumn.ChatID, MessageType.RichText, JObject.FromObject(richMessage).ToString());
                }
            }
            else if (richesText.Count > 0)
            {
                string message = string.Empty;
                foreach (RichMessageContentModel item in richesText)
                {
                    message += item.Content;
                }
                ChatService.SendMessage(chatColumn.ChatID, MessageType.Text, message);
            }

            Dispatcher.Invoke(delegate
            {
                chatColumn.IsUsable = true;
                chatColumn.Flow.Blocks.Clear();
                TransferringData(typeof(ChatMain), DataPassingType.MessageFocus, chatColumn.ChatID);
            });
        }
        #endregion
    }
}
