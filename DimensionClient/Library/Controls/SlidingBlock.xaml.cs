using System.Windows;
using System.Windows.Controls;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// SlidingBlock.xaml 的交互逻辑
    /// </summary>
    public partial class SlidingBlock : UserControl
    {
        public bool SlidingBlockState
        {
            get => (bool)GetValue(SlidingBlockStateProperty);
            set => SetValue(SlidingBlockStateProperty, value);
        }

        // Using a DependencyProperty as the backing store for SlidingBlockState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SlidingBlockStateProperty =
            DependencyProperty.Register("SlidingBlockState", typeof(bool), typeof(SlidingBlock), new PropertyMetadata(true, OnSlidingBlockStateChanged));

        public static void OnSlidingBlockStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidingBlock slidingBlock = (SlidingBlock)d;
            if (Convert.ToBoolean(e.NewValue))
            {
                slidingBlock.brdSelect.Tag = "1";
                slidingBlock.ccnLeft.Tag = "1";
                slidingBlock.ccnRight.Tag = "0";
            }
            else
            {
                slidingBlock.brdSelect.Tag = "0";
                slidingBlock.ccnLeft.Tag = "0";
                slidingBlock.ccnRight.Tag = "1";
            }
        }

        public object LeftContent
        {
            get => GetValue(LeftContentProperty);
            set => SetValue(LeftContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for LeftContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register("LeftContent", typeof(object), typeof(SlidingBlock), new PropertyMetadata(null, OnLeftContentChanged));

        public static void OnLeftContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidingBlock slidingBlock = (SlidingBlock)d;
            slidingBlock.ccnLeft.Content = slidingBlock.LeftContent;
        }

        public object RightContent
        {
            get => GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for RightContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register("RightContent", typeof(object), typeof(SlidingBlock), new PropertyMetadata(null, OnRightContentChanged));

        public static void OnRightContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SlidingBlock slidingBlock = (SlidingBlock)d;
            slidingBlock.ccnRight.Content = slidingBlock.RightContent;
        }

        public SlidingBlock()
        {
            InitializeComponent();
            tolMain.IsChecked = true;
        }

        private void TolMain_Checked(object sender, RoutedEventArgs e)
        {
            SlidingBlockState = true;
        }

        private void TolMain_Unchecked(object sender, RoutedEventArgs e)
        {
            SlidingBlockState = false;
        }
    }
}
