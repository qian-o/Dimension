using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DimensionClient.Component.Windows
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly Storyboard noPasswordLoginShow = new();
        private readonly Storyboard noPasswordLoginHide = new();
        private readonly Storyboard passwordLoginShow = new();
        private readonly Storyboard passwordLoginHide = new();
        private bool closeState;

        public LoginWindow()
        {
            InitializeComponent();

            ClassHelper.LoginWindow = this;
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
                    Duration = new TimeSpan(0, 0, 0, 0, 300),
                };
                doubleAnimation.Completed += delegate
                {
                    closeState = true;
                    Close();
                };
                BeginAnimation(OpacityProperty, doubleAnimation);
            }
        }

        private void LoginMain_Loaded(object sender, RoutedEventArgs e)
        {
            #region 免密码登录_显示
            DoubleAnimation doubleScaleX_NoPasswordLoginShow = new()
            {
                From = 0.9,
                To = 1,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleScaleY_NoPasswordLoginShow = new()
            {
                From = 0.9,
                To = 1,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleY_NoPasswordLoginShow = new()
            {
                From = 24,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            noPasswordLoginShow.Children.Add(doubleScaleX_NoPasswordLoginShow);
            noPasswordLoginShow.Children.Add(doubleScaleY_NoPasswordLoginShow);
            noPasswordLoginShow.Children.Add(doubleY_NoPasswordLoginShow);
            Storyboard.SetTargetProperty(doubleScaleX_NoPasswordLoginShow, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(doubleScaleY_NoPasswordLoginShow, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTargetProperty(doubleY_NoPasswordLoginShow, new PropertyPath("RenderTransform.Children[1].Y"));
            #endregion
            #region 免密码登录_隐藏
            DoubleAnimation doubleScaleX_NoPasswordLoginHide = new()
            {
                From = 1,
                To = 0.9,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleScaleY_NoPasswordLoginHide = new()
            {
                From = 1,
                To = 0.9,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleY_NoPasswordLoginHide = new()
            {
                From = 0,
                To = 24,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            noPasswordLoginHide.Children.Add(doubleScaleX_NoPasswordLoginHide);
            noPasswordLoginHide.Children.Add(doubleScaleY_NoPasswordLoginHide);
            noPasswordLoginHide.Children.Add(doubleY_NoPasswordLoginHide);
            Storyboard.SetTargetProperty(doubleScaleX_NoPasswordLoginHide, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(doubleScaleY_NoPasswordLoginHide, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTargetProperty(doubleY_NoPasswordLoginHide, new PropertyPath("RenderTransform.Children[1].Y"));
            #endregion
            #region 密码登录_显示
            DoubleAnimation doubleScaleX_PasswordLoginShow = new()
            {
                From = 1,
                To = 0.9,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleScaleY_PasswordLoginShow = new()
            {
                From = 1,
                To = 0.9,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleY_PasswordLoginShow = new()
            {
                From = 0,
                To = -400,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleOpacity_PasswordLoginShow = new()
            {
                From = 1,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 200),
                BeginTime = new TimeSpan(0, 0, 0, 0, 200)
            };
            passwordLoginShow.Children.Add(doubleScaleX_PasswordLoginShow);
            passwordLoginShow.Children.Add(doubleScaleY_PasswordLoginShow);
            passwordLoginShow.Children.Add(doubleY_PasswordLoginShow);
            passwordLoginShow.Children.Add(doubleOpacity_PasswordLoginShow);
            Storyboard.SetTargetProperty(doubleScaleX_PasswordLoginShow, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(doubleScaleY_PasswordLoginShow, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTargetProperty(doubleY_PasswordLoginShow, new PropertyPath("RenderTransform.Children[1].Y"));
            Storyboard.SetTargetProperty(doubleOpacity_PasswordLoginShow, new PropertyPath("Opacity"));
            #endregion
            #region 密码登录_隐藏
            DoubleAnimation doubleScaleX_PasswordLoginHide = new()
            {
                From = 0.9,
                To = 1,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleScaleY_PasswordLoginHide = new()
            {
                From = 0.9,
                To = 1,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleY_PasswordLoginHide = new()
            {
                From = -400,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, 400)
            };
            DoubleAnimation doubleOpacity_PasswordLoginHide = new()
            {
                From = 0,
                To = 1,
                Duration = new TimeSpan(0, 0, 0, 0, 200),
                BeginTime = new TimeSpan(0, 0, 0, 0, 200)
            };
            passwordLoginHide.Children.Add(doubleScaleX_PasswordLoginHide);
            passwordLoginHide.Children.Add(doubleScaleY_PasswordLoginHide);
            passwordLoginHide.Children.Add(doubleY_PasswordLoginHide);
            passwordLoginHide.Children.Add(doubleOpacity_PasswordLoginHide);
            Storyboard.SetTargetProperty(doubleScaleX_PasswordLoginHide, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(doubleScaleY_PasswordLoginHide, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTargetProperty(doubleY_PasswordLoginHide, new PropertyPath("RenderTransform.Children[1].Y"));
            Storyboard.SetTargetProperty(doubleOpacity_PasswordLoginHide, new PropertyPath("Opacity"));
            #endregion

            ClassHelper.MessageHint += ClassHelper_MessageHint;
        }

        private void LoginMain_Unloaded(object sender, RoutedEventArgs e)
        {
            ClassHelper.MessageHint -= ClassHelper_MessageHint;
        }

        private void ClassHelper_MessageHint(int messageType, string message)
        {
            Dispatcher.Invoke(delegate
            {
                ClassHelper.AlertMessageBox(this, ClassHelper.MessageBoxType.Inform, message);
            });
        }

        #region 切换到短信登录(鼠标,触控)
        private void TxbNoPasswordLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbNoPasswordLogin_PointerUp();
            }
        }
        private void TxbNoPasswordLogin_TouchUp(object sender, TouchEventArgs e)
        {
            TxbNoPasswordLogin_PointerUp();
        }
        #endregion

        #region 执行事件
        private void TxbNoPasswordLogin_PointerUp()
        {
            if (txbNoPasswordLogin.Tag.ToString() == "false")
            {
                noPasswordLoginShow.Begin(grdNoPasswordLogin);
                passwordLoginShow.Begin(grdPasswordLogin);

                txbNoPasswordLogin.Tag = "true";
                txbNoPasswordLogin.Text = ClassHelper.FindResource<string>("PasswordLogin");
                conNoPassword.txtPhoneNumber.Text = conPassword.txtLoginName.Text;
            }
            else
            {
                noPasswordLoginHide.Begin(grdNoPasswordLogin);
                passwordLoginHide.Begin(grdPasswordLogin);

                txbNoPasswordLogin.Tag = "false";
                txbNoPasswordLogin.Text = ClassHelper.FindResource<string>("NoPasswordLogin");
                conPassword.txtLoginName.Text = conNoPassword.txtPhoneNumber.Text;
            }
        }
        public void LoginSuccess(GetUserInfoModel getUserInfo)
        {
            ClassHelper.commonView.NickName = getUserInfo.NickName;
            ClassHelper.commonView.PhoneNumber = getUserInfo.PhoneNumber;
            ClassHelper.commonView.HeadPortrait = getUserInfo.HeadPortrait;
            ClassHelper.commonView.OnLine = getUserInfo.OnLine;
            SignalRClientHelper.InitializeConnection();
            Dispatcher.Invoke(delegate
            {
                MainWindow mainWindow = new();
                mainWindow.Show();
                Close();
            });
        }
        #endregion
    }
}