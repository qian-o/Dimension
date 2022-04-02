using Dimension.Domain;
using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using DimensionClient.Service.Call;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// CallMenu.xaml 的交互逻辑
    /// </summary>
    public partial class CallMenu : UserControl
    {
        private readonly Storyboard callMenuShow = ClassHelper.FindResource<Storyboard>("CallMenu_Show");
        private readonly Storyboard callMenuHide = ClassHelper.FindResource<Storyboard>("CallMenu_Hide");
        private string friendID = string.Empty;
        private bool isShow = false;

        public CallMenu()
        {
            InitializeComponent();

            ClassHelper.DataPassingChanged += ClassHelper_DataPassingChanged;
        }

        #region 隐藏菜单(鼠标,触控)
        private void GrdMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                GrdMain_PointerUp();
            }
        }
        private void GrdMain_TouchUp(object sender, TouchEventArgs e)
        {
            GrdMain_PointerUp();
        }
        #endregion

        private void BtnHandle_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ThreadPool.QueueUserWorkItem(VoiceCall, button.Tag);
            BeginStoryboard(callMenuHide);
            isShow = false;
        }

        private void ClassHelper_DataPassingChanged(ClassHelper.DataPassingType dataType, object data)
        {
            if (dataType == ClassHelper.DataPassingType.SelectCall)
            {
                friendID = data.ToString();
                Dispatcher.Invoke(delegate
                {
                    BeginStoryboard(callMenuShow);
                });
                isShow = true;
            }
        }

        #region 执行事件
        private void GrdMain_PointerUp()
        {
            if (isShow)
            {
                BeginStoryboard(callMenuHide);
                isShow = false;
            }
        }
        private void VoiceCall(object data)
        {
            ClassHelper.ShowMask(true);

            CallType call = data.ToString() == "Voice" ? CallType.Voice : CallType.Video;
            List<string> member = new()
            {
                ClassHelper.UserID,
                friendID
            };
            if (CallService.CreateCall(member, call, out string roomID))
            {
                if (CallService.GetRoomKey(roomID, out GetRoomKeyModel roomKey))
                {
                    if (ClassHelper.CreatingCallManagement(roomID, roomKey, call, member, true))
                    {
                        ClassHelper.CallViewManager.Initialize();
                    }
                }
            }

            ClassHelper.ShowMask(false);
        }
        #endregion
    }
}
