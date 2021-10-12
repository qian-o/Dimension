using DimensionClient.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ImageMedia.xaml 的交互逻辑
    /// </summary>
    public partial class ImageMedia : UserControl
    {
        public Uri ImageUri
        {
            get => (Uri)GetValue(ImageUriProperty);
            set => SetValue(ImageUriProperty, value);
        }
        public BitmapSource ImageData
        {
            get => (BitmapSource)GetValue(ImageDataProperty);
            set => SetValue(ImageDataProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(Uri), typeof(ImageMedia), new PropertyMetadata(null, OnImageUriChanged));
        // Using a DependencyProperty as the backing store for ImageData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageDataProperty =
            DependencyProperty.Register("ImageData", typeof(BitmapSource), typeof(ImageMedia), new PropertyMetadata(null, OnImageDataChanged));

        public static void OnImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageMedia image = d as ImageMedia;
            Uri uri = e.NewValue as Uri;
            if (uri.LocalPath.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
            {
                AnimationBehavior.SetSourceUri(image.imgSource, uri);
                AnimationBehavior.SetRepeatBehavior(image.imgSource, RepeatBehavior.Forever);
            }
            else
            {
                image.imgSource.Source = new BitmapImage(uri);
            }
        }
        public static void OnImageDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageMedia image = d as ImageMedia;
            image.imgSource.Visibility = Visibility.Visible;
            image.imgSource.Source = e.NewValue as BitmapSource;
        }

        public ImageMedia()
        {
            InitializeComponent();
        }
    }
}
