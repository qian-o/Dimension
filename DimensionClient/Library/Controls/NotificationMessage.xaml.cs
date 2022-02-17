using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// NotificationMessage.xaml 的交互逻辑
    /// </summary>
    public partial class NotificationMessage : UserControl
    {
        public readonly Storyboard storyboard = new();
        public NotificationMessage(string title, string message)
        {
            InitializeComponent();

            txbTitle.Text = title;
            txbMessage.Text = message;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubleScaleY = new()
            {
                From = 1,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 300)
            };
            DoubleAnimation doubleOpacity = new()
            {
                From = 1,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 300)
            };
            storyboard.Children.Add(doubleScaleY);
            storyboard.Children.Add(doubleOpacity);
            Storyboard.SetTargetProperty(doubleScaleY, new PropertyPath("LayoutTransform.ScaleY"));
            Storyboard.SetTargetProperty(doubleOpacity, new PropertyPath("Opacity"));
        }

        #region 关闭通知(鼠标,触控)
        private void TxbClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbClose_PointerUp();
            }
        }
        private void TxbClose_TouchUp(object sender, TouchEventArgs e)
        {
            TxbClose_PointerUp();
        }
        #endregion

        #region 执行事件
        private void TxbClose_PointerUp()
        {
            storyboard.Begin(this);
            txbClose.IsEnabled = false;
        }
        #endregion
    }
}
