using DimensionClient.Common;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// MessageOwn.xaml 的交互逻辑
    /// </summary>
    public partial class MessageOwn : UserControl
    {
        public MessageOwn()
        {
            InitializeComponent();
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            ChatMessagesModel chatMessages = DataContext as ChatMessagesModel;
            if (chatMessages.LoadFirst)
            {
                DoubleAnimation doubleAnimationScale = new()
                {
                    From = 0.6,
                    To = 1,
                    Duration = new TimeSpan(0, 0, 0, 0, 800),
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                };
                DoubleAnimation doubleAnimationY = new()
                {
                    From = 20,
                    To = 0,
                    Duration = new TimeSpan(0, 0, 0, 0, 800),
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                };
                DoubleAnimation doubleAnimationOpacity = new()
                {
                    From = 0,
                    To = 1,
                    Duration = new TimeSpan(0, 0, 0, 0, 800),
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                };
                stfLoaded.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimationScale);
                stfLoaded.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimationScale);
                ttfLoaded.BeginAnimation(TranslateTransform.YProperty, doubleAnimationY);
                BeginAnimation(OpacityProperty, doubleAnimationOpacity);
                chatMessages.LoadFirst = false;
            }
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ChatMessagesModel chatMessages)
            {
                imgHead.DataContext = chatMessages.SenderID;
                txbTime.Text = chatMessages.CreateTime.ToString("t", ClassHelper.cultureInfo);
                switch (chatMessages.MessageType)
                {
                    case ClassHelper.MessageType.Text:
                        txbContent.Visibility = Visibility.Visible;
                        txbContent.Text = chatMessages.MessageContent;
                        break;
                    case ClassHelper.MessageType.Voice:
                        break;
                    case ClassHelper.MessageType.File:
                        {
                            FileModel fileModel = JsonConvert.DeserializeObject<FileModel>(chatMessages.MessageContent);
                            switch (fileModel.FileType)
                            {
                                case ClassHelper.FileType.Image:
                                    conImageMedia.Visibility = Visibility.Visible;
                                    if (fileModel.FileWidth < 440 && fileModel.FileHeight < 440)
                                    {
                                        conImageMedia.Width = fileModel.FileWidth;
                                        conImageMedia.Height = fileModel.FileHeight;
                                    }
                                    else
                                    {
                                        double ratios = fileModel.FileWidth / 440;
                                        conImageMedia.Width = 440;
                                        conImageMedia.Height = fileModel.FileHeight / ratios;
                                    }
                                    if (fileModel.FileName.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                                    {
                                        conImageMedia.SetBinding(ImageMedia.ImageUriProperty, new Binding { Converter = ClassHelper.FindResource<IValueConverter>("SourceOnlineConvert") });
                                    }
                                    else
                                    {
                                        conImageMedia.SetBinding(ImageMedia.ImageDataProperty, new Binding { Converter = ClassHelper.FindResource<IValueConverter>("ImageSourceOnlineConvert"), ConverterParameter = 600 });
                                    }
                                    conImageMedia.DataContext = fileModel.FileName;
                                    break;
                                case ClassHelper.FileType.Word:
                                    break;
                                case ClassHelper.FileType.Excel:
                                    break;
                                case ClassHelper.FileType.PPT:
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ClassHelper.MessageType.VoiceTalk:
                        break;
                    case ClassHelper.MessageType.VideoTalk:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
