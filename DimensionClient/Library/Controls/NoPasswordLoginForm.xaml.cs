using DimensionClient.Common;
using DimensionClient.Dao.LoginUser;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.UserManager;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// NoPasswordLoginForm.xaml 的交互逻辑
    /// </summary>
    public partial class NoPasswordLoginForm : UserControl
    {
        private readonly NoPasswordLoginViewModel noPasswordLoginView;
        private DoubleAnimation doubleAnimation;

        public NoPasswordLoginForm()
        {
            InitializeComponent();
            noPasswordLoginView = DataContext as NoPasswordLoginViewModel;
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
        }

        #region 获取验证码(鼠标,触控)
        private void TxbGetCode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                TxbGetCode_PointerUp();
            }
        }
        private void TxbGetCode_TouchUp(object sender, TouchEventArgs e)
        {
            TxbGetCode_PointerUp();
        }
        #endregion

        private void BtnNoPasswordLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(noPasswordLoginView.PhoneNumber))
            {
                txtPhoneNumber.Focus();
                ttfPhoneNumber.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
            }
            else if (string.IsNullOrEmpty(noPasswordLoginView.Code))
            {
                txtCode.Focus();
                ttfCode.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(NoPasswordLoginRequest);
            }
        }

        #region 执行事件
        private void TxbGetCode_PointerUp()
        {
            if (!string.IsNullOrEmpty(noPasswordLoginView.PhoneNumber) && Regex.IsMatch(noPasswordLoginView.PhoneNumber, ClassHelper.phoneVerify))
            {
                ThreadPool.QueueUserWorkItem(GetCodeRequest);
            }
            else
            {
                txtPhoneNumber.Focus();
                ttfPhoneNumber.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
            }
        }
        private void GetCodeRequest(object data)
        {
            if (UserManagerService.GetVerificationCode(noPasswordLoginView.PhoneNumber))
            {
                Dispatcher.Invoke(delegate
                {
                    txbGetCode.IsEnabled = false;
                });
                for (int i = 60; i > 0; i--)
                {
                    noPasswordLoginView.GetCode = i.ToString(ClassHelper.cultureInfo);
                    Thread.Sleep(950);
                }
                noPasswordLoginView.GetCode = ClassHelper.FindResource<string>("GetCode");
                Dispatcher.Invoke(delegate
                {
                    txbGetCode.IsEnabled = true;
                });
            }
        }
        private void NoPasswordLoginRequest(object data)
        {
            Dispatcher.Invoke(delegate
            {
                btnNoPasswordLogin.IsEnabled = false;
            });
            if (UserManagerService.PhoneNumberLogin(noPasswordLoginView.PhoneNumber, noPasswordLoginView.Code, out UserLoginModel userLoginModel))
            {
                ClassHelper.UserID = userLoginModel.UserID;
                ClassHelper.Token = userLoginModel.Token;
                if (UserManagerService.GetUserInfo(out GetUserInfoModel getUserInfoModel))
                {
                    LoginUserDAO.UpdateLoginUser(new LoginUserModel
                    {
                        UserID = userLoginModel.UserID,
                        NickName = getUserInfoModel.NickName,
                        LoginName = noPasswordLoginView.PhoneNumber,
                        HeadPortrait = getUserInfoModel.HeadPortrait,
                        Password = null
                    });
                }
                Thread.Sleep(1000);
                ClassHelper.LoginWindow.LoginSuccess(getUserInfoModel);
            }
            Dispatcher.Invoke(delegate
            {
                btnNoPasswordLogin.IsEnabled = true;
            });
        }
        #endregion
    }
}