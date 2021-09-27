using DimensionClient.Common;
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
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using static DimensionClient.Common.ClassHelper;
using static DimensionClient.Common.DisplayDevice;
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

                Image imgMessage = new()
                {
                    MaxHeight = 100,
                    MaxWidth = 100
                };
                imgMessage.MouseLeftButtonDown += ImgMessage_MouseLeftButtonDown;
                imgMessage.TouchDown += ImgMessage_TouchDown;

                if (Clipboard.ContainsFileDropList())
                {
                    string file = Clipboard.GetFileDropList()[0];
                    if (File.Exists(file))
                    {
                        BitmapImage bitmap = new(new Uri(file, UriKind.Absolute));
                        string ext = new FileInfo(file).Extension.ToLower(cultureInfo);
                        if (ext.Contains("gif"))
                        {
                            ImageBehavior.SetAnimatedSource(imgMessage, bitmap);
                        }
                        else
                        {
                            imgMessage.Source = bitmap;
                        }
                        imgMessage.Tag = ext;
                    }
                }
                else if (Clipboard.GetImage() is BitmapSource bitmap)
                {
                    imgMessage.Source = bitmap;
                }
                _ = new InlineUIContainer(imgMessage, rtbMessage.Selection.End.GetPositionAtOffset(0));
                if (rtbMessage.Selection.End.GetPositionAtOffset(3) != null)
                {
                    rtbMessage.Selection.Select(rtbMessage.Selection.End.GetPositionAtOffset(3), rtbMessage.Selection.End.GetPositionAtOffset(3));
                }
            }
        }

        #region 富文本中查看图片(鼠标,触控)
        private void ImgMessage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                ImgMessage_PointerDown(sender);
            }
        }
        private void ImgMessage_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Timestamp - lastTouch < 300)
            {
                ImgMessage_PointerDown(sender);
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
        private static void ImgMessage_PointerDown(object sender)
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

            List<DisplayInfoModel> displays = new();
            DisplayDevice d = new();
            d.Cb = Marshal.SizeOf(d);
            for (uint id = 0; GetDisplayDevices(null, id, ref d, 0); id++)
            {
                if (d.DeviceState.HasFlag(DisplayDeviceState.AttachedToDesktop))
                {
                    if (GetDisplaySettings(d.DeviceName, currentSettings, out DevMode dEVMODE))
                    {
                        DisplayInfoModel displayInfo = new()
                        {
                            DisplayWidth = dEVMODE.DmPelsWidth,
                            DisplayHeight = dEVMODE.DmPelsHeight,
                            DisplayLeft = dEVMODE.DmPositionX,
                            DisplayTop = dEVMODE.DmPositionY,
                            MainDisplay = d.DeviceState.HasFlag(DisplayDeviceState.PrimaryDevice)
                        };
                        displays.Add(displayInfo);
                    }
                }
                d.Cb = Marshal.SizeOf(d);
            }
            int actualLeft = 0;
            int actualTop = 0;
            int actualRight = 0;
            int actualBottom = 0;
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
            }
            int actualWidth = Math.Abs(actualLeft) + Math.Abs(actualRight);
            int actualHeight = Math.Abs(actualTop) + Math.Abs(actualBottom);
            Bitmap bitmap = new(actualWidth, actualHeight);
            Graphics.FromImage(bitmap).CopyFromScreen(actualLeft, actualTop, 0, 0, new System.Drawing.Size(actualWidth, actualHeight));

            Dispatcher.Invoke(delegate
            {
                Screenshots screenshots = new(bitmap, actualLeft, actualTop);
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
                                if (con.Child is Image image)
                                {
                                    Task task = new(() =>
                                    {
                                        MultipartFormDataContent dataContent = new();
                                        Dispatcher.Invoke(delegate
                                        {
                                            using MemoryStream memoryStream = new();
                                            string extend;
                                            if (image.Tag != null)
                                            {
                                                extend = image.Tag.ToString();
                                                BitmapImage bitmap = image.Source is BitmapImage bitmapImage ? bitmapImage : (BitmapImage)ImageBehavior.GetAnimatedSource(image);
                                                new FileStream(bitmap.UriSource.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read).CopyTo(memoryStream);
                                            }
                                            else
                                            {
                                                extend = ".bmp";
                                                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                                                bitmapEncoder.Frames.Add(BitmapFrame.Create(image.Source as BitmapSource));
                                                bitmapEncoder.Save(memoryStream);
                                            }
                                            dataContent.Add(new ByteArrayContent(memoryStream.ToArray()), "file", $"{GetRandomString(10)}{extend}");
                                            memoryStream.Close();
                                        });
                                        if (ServerUpload($"{servicePath}/api/Attachment/UploadAttachment", dataContent, out string fileName))
                                        {
                                            FileModel fileModel = new()
                                            {
                                                FileType = FileType.Image,
                                                FileName = fileName
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
