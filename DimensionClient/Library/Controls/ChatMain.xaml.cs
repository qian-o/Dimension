using DimensionClient.Component.Windows;
using DimensionClient.Library.CustomControls;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static DimensionClient.Common.ClassHelper;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ChatMain.xaml 的交互逻辑
    /// </summary>
    public partial class ChatMain : UserControl
    {
        private readonly ChatMainViewModel chatMainData;
        private ChatItem chatItem;
        public ChatMain()
        {
            InitializeComponent();

            chatMainData = DataContext as ChatMainViewModel;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataPassingChanged += ClassHelper_DataPassingChanged;
            ThreadPool.QueueUserWorkItem(Load);
        }

        private void UserControlMain_Unloaded(object sender, RoutedEventArgs e)
        {
            DataPassingChanged -= ClassHelper_DataPassingChanged;
        }

        private void RtbMessage_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                e.CancelCommand();

                ChatImage chatImage = new()
                {
                    MaxHeight = 100,
                    MaxWidth = 100
                };

                if (Clipboard.ContainsFileDropList())
                {
                    string file = Clipboard.GetFileDropList()[0];
                    if (File.Exists(file))
                    {
                        chatImage.PathUri = new Uri(file, UriKind.Absolute);
                    }
                }
                else if (Clipboard.GetData(DataFormats.Bitmap) is InteropBitmap bitmap)
                {
                    using MemoryStream memory = new();
                    BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                    bitmapEncoder.Save(memory);
                    byte[] data = memory.ToArray();
                    string path = Path.Combine(Path.GetTempPath(), $"{GetCacheFileName(data)}.bmp");
                    if (!File.Exists(path))
                    {
                        using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.Write);
                        stream.Write(data, 0, data.Length);
                        stream.Close();
                    }
                    chatImage.PathUri = new Uri(path, UriKind.Absolute);
                }

                bool tagEnd = rtbMessage.Selection.End.GetPositionAtOffset(2) == null || rtbMessage.Selection.End.GetPositionAtOffset(2).GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None;
                _ = new InlineUIContainer(chatImage, rtbMessage.Selection.Start.GetPositionAtOffset(0));
                if (tagEnd)
                {
                    rtbMessage.Selection.Select(rtbMessage.Document.ContentEnd, rtbMessage.Document.ContentEnd);
                }
            }
        }

        #region 截图(鼠标,触控)
        private void TxbScreenCapture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbScreenCapture_PointerUp();
            }
        }
        private void TxbScreenCapture_TouchUp(object sender, TouchEventArgs e)
        {
            TxbScreenCapture_PointerUp();
        }
        #endregion

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter == null)
            {
                if (e.Command == EditingCommands.EnterParagraphBreak)
                {
                    chatItem.Send();
                }
                e.Handled = true;
            }
        }

        private void RtbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                EditingCommands.EnterParagraphBreak.Execute(1, rtbMessage);
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            chatItem.Send();
        }

        private void ClassHelper_DataPassingChanged(DataPassingType dataType, object data)
        {
            if (dataType == DataPassingType.Paste)
            {
                rtbMessage.Paste();
            }
            else if (dataType == DataPassingType.MessageFocus)
            {
                if (data.ToString() == chatMainData.ChatID)
                {
                    rtbMessage.Focus();
                }
            }
            else if (dataType == DataPassingType.SelectMessage)
            {
                chatItem = data as ChatItem;
                Visibility = Visibility.Visible;
                ChatColumnInfoModel chatColumnInfo = chatItem.DataContext as ChatColumnInfoModel;
                chatMainData.ChatID = chatColumnInfo.ChatID;
                txbFriendNickName.SetBinding(TextBlock.TextProperty, new Binding { Source = chatColumnInfo, Path = new PropertyPath(string.IsNullOrEmpty(chatColumnInfo.RemarkName) ? nameof(chatColumnInfo.NickName) : nameof(chatColumnInfo.RemarkName)) });
                rtbMessage.SetBinding(IsEnabledProperty, new Binding { Source = chatColumnInfo, Path = new PropertyPath(nameof(chatColumnInfo.IsUsable)) });
                btnSend.SetBinding(IsEnabledProperty, new Binding { Source = chatColumnInfo, Path = new PropertyPath(nameof(chatColumnInfo.IsUsable)) });
                if (chatColumnInfo.Flow == null)
                {
                    chatColumnInfo.Flow = new FlowDocument();
                    chatColumnInfo.Flow.Loaded += Flow_Loaded;
                }
                rtbMessage.Document = chatColumnInfo.Flow;
                brdChat.Child = chatItem.MasterChat;
            }
        }

        private void Flow_Loaded(object sender, RoutedEventArgs e)
        {
            rtbMessage.Selection.Select(rtbMessage.Document.ContentEnd, rtbMessage.Document.ContentEnd);
            rtbMessage.Focus();
        }

        #region 执行事件
        private void Load(object data)
        {
            List<EmojiModel> emojis = new();

            ResourceManager resourceManager = new($"{Assembly.GetExecutingAssembly().GetName().Name}.g", Assembly.GetExecutingAssembly());
            ResourceSet resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry dictionary in resources)
            {
                if (dictionary.Key.ToString().Contains("library/image/emoji/"))
                {
                    emojis.Add(new EmojiModel
                    {
                        EmojiKey = dictionary.Key.ToString(),
                        ResourceUri = $"pack://application:,,,/{dictionary.Key}"
                    });
                }
            }

            chatMainData.Emojis = emojis;
        }
        private static void TxbScreenCapture_PointerUp()
        {
            TransferringData(typeof(MainWindow), DataPassingType.ScreenCapture, null);
        }
        #endregion
    }
}
