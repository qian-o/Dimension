using DimensionClient.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XamlAnimatedGif;

namespace DimensionClient.Library.CustomControls
{
    public class SerializableImage : Image
    {
        public Uri PathUri
        {
            get => (Uri)GetValue(PathUriProperty);
            set => SetValue(PathUriProperty, value);
        }

        // Using a DependencyProperty as the backing store for PathUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathUriProperty =
            DependencyProperty.Register("PathUri", typeof(Uri), typeof(SerializableImage), new PropertyMetadata(null));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ImageSource Source
        {
            get => base.Source;
            set => base.Source = value;
        }

        public SerializableImage()
        {
            Loaded += SerializableImage_Loaded;
            Unloaded += SerializableImage_Unloaded;
        }

        private void SerializableImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PathUri != null)
            {
                if (PathUri.LocalPath.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                {
                    AnimationBehavior.SetSourceUri(this, PathUri);
                }
                else
                {
                    Source = new BitmapImage(PathUri);
                }
            }
        }

        private void SerializableImage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (PathUri != null)
            {
                if (PathUri.LocalPath.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
                {
                    AnimationBehavior.SetSourceUri(this, null);
                }
            }
        }
    }
}