using DimensionClient.Component.Windows;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.Chat;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static DimensionClient.Common.ClassHelper;
using Image = System.Windows.Controls.Image;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ChatMain.xaml 的交互逻辑
    /// </summary>
    public partial class ChatMain : UserControl
    {
        private readonly ChatMainViewModel chatMainData;
        private int lastTouch;
        public ChatMain()
        {
            InitializeComponent();

            chatMainData = DataContext as ChatMainViewModel;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            DataPassingChanged += ClassHelper_DataPassingChanged;
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

                ImageMedia imageMedia = new()
                {
                    MaxHeight = 100,
                    MaxWidth = 100
                };
                imageMedia.MouseLeftButtonDown += ImageMedia_MouseLeftButtonDown;
                imageMedia.TouchDown += ImageMedia_TouchDown;

                if (Clipboard.ContainsFileDropList())
                {
                    string file = Clipboard.GetFileDropList()[0];
                    if (File.Exists(file))
                    {
                        imageMedia.ImageUri = new Uri(file, UriKind.Absolute);
                    }
                }
                else if (Clipboard.GetData(DataFormats.Bitmap) is InteropBitmap bitmap)
                {
                    imageMedia.ImageData = bitmap;
                }
                bool tagEnd = rtbMessage.Selection.End.GetPositionAtOffset(2) == null || rtbMessage.Selection.End.GetPositionAtOffset(2).GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None;
                _ = new InlineUIContainer(imageMedia, rtbMessage.Selection.Start.GetPositionAtOffset(0));
                if (tagEnd)
                {
                    rtbMessage.Selection.Select(rtbMessage.Document.ContentEnd, rtbMessage.Document.ContentEnd);
                }
            }
        }

        #region 富文本中查看图片(鼠标,触控)
        private void ImageMedia_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                ImageMedia_PointerDown(sender);
            }
        }
        private void ImageMedia_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Timestamp - lastTouch < 300)
            {
                ImageMedia_PointerDown(sender);
            }
            else
            {
                lastTouch = e.Timestamp;
            }
        }
        #endregion

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
                    ThreadPool.QueueUserWorkItem(SendMessage);
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
            ThreadPool.QueueUserWorkItem(SendMessage);
        }

        private void ClassHelper_DataPassingChanged(object data)
        {
            Visibility = Visibility.Visible;

            ChatItem chatItem = data as ChatItem;
            ChatColumnInfoModel chatColumnInfo = chatItem.DataContext as ChatColumnInfoModel;
            chatMainData.ChatID = chatColumnInfo.ChatID;
            txbFriendNickName.SetBinding(TextBlock.TextProperty, new Binding { Source = chatColumnInfo, Path = new PropertyPath(string.IsNullOrEmpty(chatColumnInfo.RemarkName) ? nameof(chatColumnInfo.NickName) : nameof(chatColumnInfo.RemarkName)) });
            brdChat.Child = chatItem.MasterChat;
        }

        #region 执行事件
        private static void ImageMedia_PointerDown(object sender)
        {
            Image image = sender as Image;
            Console.WriteLine(image);
        }
        private void TxbScreenCapture_PointerUp()
        {
            ThreadPool.QueueUserWorkItem(ScreenCapture);
        }
        private void ScreenCapture(object data)
        {
            Dispatcher.Invoke(delegate
            {
                txbScreenCapture.IsEnabled = false;
            });

            List<DisplayInfoModel> displays = GetDisplayInfos();
            int actualLeft = 0;
            int actualTop = 0;
            int actualRight = 0;
            int actualBottom = 0;

            double showLeft = 0;
            double showTop = 0;
            double showRight = 0;
            double showBottom = 0;
            foreach (DisplayInfoModel item in displays)
            {
                if (actualLeft > item.DisplayLeft)
                {
                    actualLeft = item.DisplayLeft;
                }
                if (actualTop > item.DisplayTop)
                {
                    actualTop = item.DisplayTop;
                }
                if (actualRight < item.DisplayLeft + item.DisplayWidth)
                {
                    actualRight = item.DisplayLeft + item.DisplayWidth;
                }
                if (actualBottom < item.DisplayTop + item.DisplayHeight)
                {
                    actualBottom = item.DisplayTop + item.DisplayHeight;
                }

                if (showLeft > item.ShowLeft)
                {
                    showLeft = item.ShowLeft;
                }
                if (showTop > item.ShowTop)
                {
                    showTop = item.ShowTop;
                }
                if (showRight < item.ShowLeft + item.ShowWidth)
                {
                    showRight = item.ShowLeft + item.ShowWidth;
                }
                if (showBottom < item.ShowTop + item.ShowHeight)
                {
                    showBottom = item.ShowTop + item.ShowHeight;
                }
            }
            int actualWidth = Math.Abs(actualLeft) + Math.Abs(actualRight);
            int actualHeight = Math.Abs(actualTop) + Math.Abs(actualBottom);

            double showWidth = Math.Abs(showLeft) + Math.Abs(showRight);
            double showHeight = Math.Abs(showTop) + Math.Abs(showBottom);
            Bitmap bitmap = new(actualWidth, actualHeight);
            Graphics.FromImage(bitmap).CopyFromScreen(actualLeft, actualTop, 0, 0, new System.Drawing.Size(actualWidth, actualHeight));

            Dispatcher.Invoke(delegate
            {
                Screenshots screenshots = new(bitmap, showLeft, showTop, showWidth, showHeight);
                screenshots.ShowDialog();
                if (screenshots.IsSave)
                {
                    rtbMessage.Paste();
                }
            });

            bitmap.Dispose();

            Dispatcher.Invoke(delegate
            {
                txbScreenCapture.IsEnabled = true;
            });
        }
        private async void SendMessage(object data)
        {
            Dispatcher.Invoke(delegate
            {
                rtbMessage.IsEnabled = false;
                btnSend.IsEnabled = false;
            });

            List<Task> uploading = new();
            Dispatcher.Invoke(delegate
            {
                foreach (Block item in rtbMessage.Document.Blocks)
                {
                    if (!string.IsNullOrEmpty(chatMainData.MessageText))
                    {
                        chatMainData.MessageText += Environment.NewLine;
                    }
                    if (item is Paragraph paragraph)
                    {
                        foreach (Inline coll in paragraph.Inlines)
                        {
                            if (coll is Run run)
                            {
                                chatMainData.MessageText += run.Text.Trim();
                            }
                            else if (coll is InlineUIContainer con)
                            {
                                if (con.Child is ImageMedia imageMedia)
                                {
                                    Task task = new(() =>
                                    {
                                        MultipartFormDataContent dataContent = new();
                                        double fileSize = 0;
                                        double fileWidth = 0;
                                        double fileHeight = 0;
                                        Dispatcher.Invoke(delegate
                                        {
                                            using MemoryStream memoryStream = new();
                                            string extend;
                                            BitmapSource bitmapSource;
                                            if (imageMedia.ImageUri != null)
                                            {
                                                extend = new FileInfo(imageMedia.ImageUri.LocalPath).Extension.ToLower(cultureInfo);
                                                File.OpenRead(imageMedia.ImageUri.LocalPath).CopyTo(memoryStream);

                                                bitmapSource = new BitmapImage(imageMedia.ImageUri);
                                            }
                                            else
                                            {
                                                extend = ".bmp";
                                                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                                                bitmapEncoder.Frames.Add(BitmapFrame.Create(imageMedia.ImageData));
                                                bitmapEncoder.Save(memoryStream);

                                                bitmapSource = imageMedia.ImageData;
                                            }
                                            dataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "file", $"{GetRandomString(10)}{extend}");

                                            fileSize = (double)memoryStream.Length / 1000 / 1000;
                                            fileWidth = bitmapSource.Width;
                                            fileHeight = bitmapSource.Height;

                                            memoryStream.Close();
                                        });
                                        if (ServerUpload($"{servicePath}/api/Attachment/UploadAttachment", dataContent, out string fileName))
                                        {
                                            FileModel fileModel = new()
                                            {
                                                FileType = FileType.Image,
                                                FileName = fileName,
                                                FileMByte = fileSize,
                                                FileWidth = fileWidth,
                                                FileHeight = fileHeight
                                            };
                                            ChatService.SendMessage(chatMainData.ChatID, MessageType.File, JObject.FromObject(fileModel).ToString());
                                        }
                                    });
                                    task.Start();
                                    uploading.Add(task);

                                    if (!string.IsNullOrEmpty(chatMainData.MessageText))
                                    {
                                        string temporary = chatMainData.MessageText;
                                        chatMainData.MessageText = string.Empty;
                                        Task temporaryTask = new(() =>
                                        {
                                            ChatService.SendMessage(chatMainData.ChatID, MessageType.Text, temporary);
                                        });
                                        temporaryTask.Start();
                                        uploading.Add(temporaryTask);
                                    }
                                }
                            }
                        }
                    }

                }
            });

            if (!string.IsNullOrEmpty(chatMainData.MessageText))
            {
                if (ChatService.SendMessage(chatMainData.ChatID, MessageType.Text, chatMainData.MessageText))
                {
                    chatMainData.MessageText = string.Empty;
                }
            }

            await Task.WhenAll(uploading);

            Dispatcher.Invoke(delegate
            {
                btnSend.IsEnabled = true;
                rtbMessage.IsEnabled = true;
                rtbMessage.Document.Blocks.Clear();
                rtbMessage.Focus();
            });
        }
        #endregion
    }
}
