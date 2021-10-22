using DimensionClient.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;

namespace DimensionClient.Library.CustomControls
{
    public class SerializableImage : Image
    {
        public static readonly DependencyProperty FileWidthProperty =
            DependencyProperty.Register("FileWidth", typeof(double), typeof(SerializableImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty FileHeightProperty =
            DependencyProperty.Register("FileHeight", typeof(double), typeof(SerializableImage), new PropertyMetadata(0.0));
        public static readonly DependencyProperty IsLoadRelativeProperty =
            DependencyProperty.Register("IsLoadRelative", typeof(bool), typeof(SerializableImage), new PropertyMetadata(true));
        public static readonly DependencyProperty PathUriProperty =
            DependencyProperty.Register("PathUri", typeof(Uri), typeof(SerializableImage), new PropertyMetadata(null, OnPathUriChanged));

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
        public Uri PathUri
        {
            get => (Uri)GetValue(PathUriProperty);
            set => SetValue(PathUriProperty, value);
        }

        public static void OnPathUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SerializableImage image = d as SerializableImage;
            image.OnLoad();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageSource Source
        {
            get => base.Source;
            set => base.Source = value;
        }

        public SerializableImage()
        {
            Unloaded += SerializableImage_Unloaded;
        }

        private void SerializableImage_Unloaded(object sender, RoutedEventArgs e)
        {
            UnLoad();
        }

        public void OnLoad()
        {
            if (PathUri != null)
            {
                if (PathUri.IsAbsoluteUri)
                {
                    if (PathUri.AbsoluteUri.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                    {
                        AnimationBehavior.SetSourceUri(this, PathUri);
                    }
                    else
                    {
                        Source = new BitmapImage(PathUri);
                    }
                    IsLoadRelative = false;
                }
                else if (IsLoadRelative)
                {
                    if (FileWidth != 0 && FileHeight != 0)
                    {
                        if (FileWidth < 440 && FileHeight < 440)
                        {
                            Width = FileWidth;
                            Height = FileHeight;
                        }
                        else
                        {
                            double ratios = FileWidth / 440;
                            Width = 440;
                            Height = FileHeight / ratios;
                        }
                        MaxHeight = double.PositiveInfinity;
                        MaxWidth = double.PositiveInfinity;
                    }
                    string path = PathUri.OriginalString;
                    Uri uri = ClassHelper.FindResource<IValueConverter>("SourceOnlineConvert").Convert(path, typeof(string), path.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture) ? null : double.IsNaN(Height) ? null : Height * 2, ClassHelper.cultureInfo) as Uri;
                    if (path.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                    {
                        AnimationBehavior.SetSourceUri(this, uri);
                    }
                    else
                    {
                        Source = new BitmapImage(uri);
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
                    AnimationBehavior.SetSourceUri(this, null);
                }
            }
        }
    }
}