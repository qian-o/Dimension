using DimensionClient.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;

namespace DimensionClient.Library.CustomControls
{
    public class ChatImage : Border
    {
        public static readonly DependencyProperty FileWidthProperty =
            DependencyProperty.Register("FileWidth", typeof(double), typeof(ChatImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty FileHeightProperty =
            DependencyProperty.Register("FileHeight", typeof(double), typeof(ChatImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty IsLoadRelativeProperty =
            DependencyProperty.Register("IsLoadRelative", typeof(bool), typeof(ChatImage), new PropertyMetadata(true));
        public static readonly DependencyProperty ActiveControlProperty =
            DependencyProperty.Register("ActiveControl", typeof(bool), typeof(ChatImage), new PropertyMetadata(false));
        public static readonly DependencyProperty PathUriProperty =
            DependencyProperty.Register("PathUri", typeof(Uri), typeof(ChatImage), new PropertyMetadata(null, OnPathUriChanged));

        public double FileWidth
        {
            get => (double)GetValue(FileWidthProperty);
            set => SetValue(FileWidthProperty, value);
        }
        public double FileHeight
        {
            get => (double)GetValue(FileHeightProperty);
            set => SetValue(FileHeightProperty, value);
        }
        public bool IsLoadRelative
        {
            get => (bool)GetValue(IsLoadRelativeProperty);
            set => SetValue(IsLoadRelativeProperty, value);
        }
        public bool ActiveControl
        {
            get => (bool)GetValue(ActiveControlProperty);
            set => SetValue(ActiveControlProperty, value);
        }
        public Uri PathUri
        {
            get => (Uri)GetValue(PathUriProperty);
            set => SetValue(PathUriProperty, value);
        }

        public static void OnPathUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatImage image = d as ChatImage;
            image.OnLoad();
        }

        #region 序列化时隐藏
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new double MaxWidth { get => base.MaxWidth; set => base.MaxWidth = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new double MaxHeight { get => base.MaxHeight; set => base.MaxHeight = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Thickness Margin { get => base.Margin; set => base.Margin = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override UIElement Child { get => base.Child; set => base.Child = value; }
        #endregion

        private readonly Image image = new();

        public ChatImage()
        {
            Grid grid = new();
            grid.Children.Add(new Border()
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 2,
                    Opacity = 0.4
                }
            });
            Border border = new()
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5)
            };
            grid.Children.Add(border);
            image.OpacityMask = new VisualBrush(border);
            grid.Children.Add(image);
            Child = grid;

            Unloaded += ChatImage_Unloaded;
        }

        private void ChatImage_Unloaded(object sender, RoutedEventArgs e)
        {
            UnLoad();
        }

        public void OnLoad()
        {
            if (PathUri != null)
            {
                if (!ActiveControl)
                {
                    Margin = new Thickness(6, 4, 6, 2);
                }
                if (PathUri.IsAbsoluteUri)
                {
                    if (PathUri.AbsoluteUri.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                    {
                        AnimationBehavior.SetSourceUri(image, PathUri);
                    }
                    else
                    {
                        image.Source = new BitmapImage(PathUri);
                    }
                    IsLoadRelative = false;
                }
                else if (IsLoadRelative)
                {
                    if (FileWidth != 0 && FileHeight != 0)
                    {
                        double maxValue = 440 - (Margin.Left + Margin.Right);
                        if (FileWidth < maxValue && FileHeight < maxValue)
                        {
                            image.MinWidth = FileWidth * 0.9;
                            image.MinHeight = FileHeight * 0.9;
                            image.MaxWidth = FileWidth;
                            image.MaxHeight = FileHeight;
                        }
                        else
                        {
                            double width = FileWidth;
                            double height = FileHeight;
                            while (width > maxValue || height > maxValue)
                            {
                                width *= 0.9;
                                height *= 0.9;
                            }
                            image.MinWidth = width * 0.9;
                            image.MinHeight = height * 0.9;
                            image.MaxWidth = width;
                            image.MaxHeight = height;
                        }
                    }
                    string path = PathUri.OriginalString;
                    Uri uri = ClassHelper.FindResource<IValueConverter>("SourceOnlineConvert").Convert(path, typeof(string), path.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture) ? null : image.MaxHeight * 2, ClassHelper.cultureInfo) as Uri;
                    if (path.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                    {
                        AnimationBehavior.SetSourceUri(image, uri);
                    }
                    else
                    {
                        image.Source = new BitmapImage(uri);
                    }
                }
            }
        }

        public void UnLoad()
        {
            if (PathUri != null)
            {
                string path = PathUri.IsAbsoluteUri ? PathUri.LocalPath : $"{ClassHelper.servicePath}/api/Attachment/GetAttachments/{PathUri.OriginalString}";
                if (path.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                {
                    AnimationBehavior.SetSourceUri(image, null);
                }
            }
        }
    }
}
