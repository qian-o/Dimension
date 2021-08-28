using DimensionClient.Common;
using DimensionClient.Dao.LoginUser;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.UserManager;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// PasswordLoginForm.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordLoginForm : UserControl
    {
        private readonly PasswordLoginViewModel passwordLoginView;
        private DoubleAnimation doubleAnimation;

        public PasswordLoginForm()
        {
            InitializeComponent();
            passwordLoginView = DataContext as PasswordLoginViewModel;
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            doubleAnimation = new()
            {
                From = -10,
                To = 0,
                Duration = new TimeSpan(0, 0, 1),
                EasingFunction = new ElasticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };
            ThreadPool.QueueUserWorkItem(Load);
        }

        private void BtnPasswordLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordLoginView.LoginName))
            {
                txtLoginName.Focus();
                ttfLoginName.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
            }
            else if (string.IsNullOrEmpty(pstPassword.Password))
            {
                pstPassword.Focus();
                ttfPassword.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(PasswordLoginRequest);
            }
        }

        #region 选择用户(鼠标,触控)
        private void GrdUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                GrdUser_PointerUp(sender);
            }
        }
        private void GrdUser_TouchUp(object sender, TouchEventArgs e)
        {
            GrdUser_PointerUp(sender);
        }
        #endregion

        #region 删除用户记录(鼠标,触控)
        private void TxbDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbDelete_PointerUp(sender);
                e.Handled = true;
            }
        }
        private void TxbDelete_TouchUp(object sender, TouchEventArgs e)
        {
            TxbDelete_PointerUp(sender);
            e.Handled = true;
        }
        #endregion

        private void ItemsControl_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        #region 执行事件
        private void Load(object data)
        {
            passwordLoginView.LoginUsers = LoginUserDAO.GetLoginUsers();
        }
        // 选择用户
        private void GrdUser_PointerUp(object sender)
        {
            if (((Grid)sender).Tag is LoginUserModel loginUserModel)
            {
                passwordLoginView.LoginName = loginUserModel.LoginName;
                pstPassword.Password = ClassHelper.AesDecrypt(loginUserModel.Password, ClassHelper.privateKey);
                passwordLoginView.RemembPassword = !string.IsNullOrEmpty(loginUserModel.Password);
                popSelect.IsOpen = false;
            }
        }
        // 删除用户记录
        private void TxbDelete_PointerUp(object sender)
        {
            TextBlock textBlock = sender as TextBlock;
            int id = Convert.ToInt32(textBlock.Tag, ClassHelper.cultureInfo);
            MessageBoxButtonModel rightButton = new()
            {
                Action = new Action(() =>
                {
                    LoginUserDAO.DeleteLoginUser(id);
                    passwordLoginView.LoginUsers = LoginUserDAO.GetLoginUsers();
                    Dispatcher.Invoke(delegate
                    {
                        txtLoginName.Text = string.Empty;
                        pstPassword.Password = string.Empty;
                        chbRememb.IsChecked = false;
                    });
                })
            };
            ClassHelper.AlertMessageBox(ClassHelper.LoginWindow, ClassHelper.MessageBoxType.Select, ClassHelper.FindResource<string>("DeleteThisRecord"), rightButton: rightButton);
            popSelect.IsOpen = true;
        }
        private void PasswordLoginRequest(object data)
        {
            Dispatcher.Invoke(delegate
            {
                txtLoginName.IsEnabled = false;
                pstPassword.IsEnabled = false;
                btnPasswordLogin.IsEnabled = false;
            });
            string password = string.Empty;
            DateTime dateTime = DateTime.Now;
            string aesKey = ClassHelper.GenerateSHA256(ClassHelper.TimeStamp(dateTime)).Substring(4, 16).ToUpper(ClassHelper.cultureInfo);
            Dispatcher.Invoke(delegate
            {
                password = pstPassword.Password;
            });
            if (UserManagerService.UserLogin(passwordLoginView.LoginName, ClassHelper.AesEncrypt(password, aesKey), dateTime, out UserLoginModel userLoginModel))
            {
                ClassHelper.UserID = userLoginModel.UserID;
                ClassHelper.Token = userLoginModel.Token;
                if (UserManagerService.GetUserInfo(out GetUserInfoModel getUserInfoModel))
                {
                    LoginUserDAO.UpdateLoginUser(new LoginUserModel
                    {
                        UserID = userLoginModel.UserID,
                        NickName = getUserInfoModel.NickName,
                        LoginName = passwordLoginView.LoginName,
                        HeadPortrait = getUserInfoModel.HeadPortrait,
                        Password = passwordLoginView.RemembPassword == true ? ClassHelper.AesEncrypt(password, ClassHelper.privateKey) : string.Empty
                    });
                }
                Thread.Sleep(1000);
                ClassHelper.LoginWindow.LoginSuccess(getUserInfoModel);
            }

            Dispatcher.Invoke(delegate
            {
                txtLoginName.IsEnabled = true;
                pstPassword.IsEnabled = true;
                btnPasswordLogin.IsEnabled = true;
            });
        }
        #endregion
    }
}
