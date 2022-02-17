using DimensionClient.Common;
using DimensionClient.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace DimensionClient.Component.Windows
{
    /// <summary>
    /// ClientMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class ClientMessageBox : Window
    {
        public ClassHelper.MessageBoxCloseType CloseType { get; set; }

        private readonly MessageBoxButtonModel _leftButton;
        private readonly MessageBoxButtonModel _rightButton;
        private bool closeState;

        public ClientMessageBox(ClassHelper.MessageBoxType messageBoxType, string message, MessageBoxButtonModel leftButton, MessageBoxButtonModel rightButton)
        {
            InitializeComponent();

            if (messageBoxType == ClassHelper.MessageBoxType.Inform)
            {
                btnLeft.Visibility = Visibility.Collapsed;
            }
            txbMessage.Text = message;
            _leftButton = leftButton;
            _rightButton = rightButton;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closeState)
            {
                e.Cancel = true;
                DoubleAnimation doubleAnimation = new()
                {
                    From = 1,
                    To = 0,
                    Duration = new TimeSpan(0, 0, 0, 0, 300)
                };
                doubleAnimation.Completed += delegate
                {
                    closeState = true;
                    Close();
                };
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        private void ClientMessageBoxMain_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr windIntPtr = new WindowInteropHelper(this).Handle;
            HwndSource hwndSource = HwndSource.FromHwnd(windIntPtr);
            hwndSource.AddHook(WndProc);

            btnLeft.Content = _leftButton.Hint;
            btnRight.Content = _rightButton.Hint;
        }

        #region 关闭窗体(鼠标,触控)
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

        private async void BtnValid_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CloseType = (ClassHelper.MessageBoxCloseType)Convert.ToInt32(button.Tag);
            grdMask.Visibility = Visibility.Visible;
            button.IsEnabled = false;
            switch (CloseType)
            {
                case ClassHelper.MessageBoxCloseType.Close:
                    break;
                case ClassHelper.MessageBoxCloseType.Left:
                    if (_leftButton.Action != null)
                    {
                        await Task.Run(_leftButton.Action);
                    }
                    break;
                case ClassHelper.MessageBoxCloseType.Right:
                    if (_rightButton.Action != null)
                    {
                        await Task.Run(_rightButton.Action);
                    }
                    break;
                default:
                    break;
            }
            Close();
        }

        #region 执行事件
        private void TxbClose_PointerUp()
        {
            CloseType = ClassHelper.MessageBoxCloseType.Close;
            Close();
        }
        // 消息钩子
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (((msg == ClassHelper.wmSystemMenu) && (wParam.ToInt32() == ClassHelper.wpSystemMenu)) || msg == 165)
            {
                handled = true;
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}
