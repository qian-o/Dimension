using DimensionClient.Common;
using DimensionClient.Models.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// DynamicImage.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicImage : UserControl
    {
        public ImageSource ImagePath
        {
            get => (ImageSource)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(ImageSource), typeof(DynamicImage), new PropertyMetadata(null, ImagePathChanged));

        public static void ImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DynamicImage dynamic = d as DynamicImage;
                DynamicImageViewModel data = ((Grid)dynamic.Content).DataContext as DynamicImageViewModel;
                ImageSource image = e.NewValue as ImageSource;
                if (!data.Show1)
                {
                    data.Source1 = image;
                    if (!image.IsFrozen)
                    {
                        image.Changed += (a, b) =>
                        {
                            if (((ImageSource)a).ToString(ClassHelper.cultureInfo) == dynamic.ImagePath.ToString(ClassHelper.cultureInfo))
                            {
                                data.Show1 = true;
                                data.Show2 = false;
                            }
                        };
                    }
                    else
                    {
                        data.Show1 = true;
                        data.Show2 = false;
                    }
                }
                else
                {
                    data.Source2 = image;
                    if (!image.IsFrozen)
                    {
                        image.Changed += (a, b) =>
                        {
                            if (((ImageSource)a).ToString(ClassHelper.cultureInfo) == dynamic.ImagePath.ToString(ClassHelper.cultureInfo))
                            {
                                data.Show1 = false;
                                data.Show2 = true;
                            }
                        };
                    }
                    else
                    {
                        data.Show1 = false;
                        data.Show2 = true;
                    }
                }
            }
        }

        public DynamicImage()
        {
            InitializeComponent();
        }
    }
}
