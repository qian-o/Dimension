using DimensionClient.Common;
using DimensionClient.Models;
using DimensionClient.Models.ResultModels;
using DimensionClient.Models.ViewModels;
using DimensionClient.Service.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace DimensionClient.Component.Windows
{
    /// <summary>
    /// InviteCall.xaml 的交互逻辑
    /// </summary>
    public partial class InviteCall : Window
    {
        private readonly Storyboard showMain = ClassHelper.FindResource<Storyboard>("InviteCallMain_Show");
        private readonly Storyboard showInfo = ClassHelper.FindResource<Storyboard>("InviteCallInfo_Show");
        private readonly Storyboard showIcon = ClassHelper.FindResource<Storyboard>("InviteCallIcon_Show");
        private readonly Storyboard showHint = ClassHelper.FindResource<Storyboard>("InviteCallHint_Show");
        private readonly Storyboard hideMain = ClassHelper.FindResource<Storyboard>("InviteCallMain_Hide");
        private readonly Storyboard hideInfo = ClassHelper.FindResource<Storyboard>("InviteCallInfo_Hide");
        private readonly Storyboard hideIcon = ClassHelper.FindResource<Storyboard>("InviteCallIcon_Hide");
        private readonly Storyboard hideHint = ClassHelper.FindResource<Storyboard>("InviteCallHint_Hide");
        private readonly InviteCallViewModel mainData;

        public InviteCall(InviteCallViewModel viewModel)
        {
            InitializeComponent();

            mainData = viewModel;
            if (viewModel.CallType == ClassHelper.CallType.Voice)
            {
                txbCallType.Text = ClassHelper.FindResource<string>("IncomingVoiceCall");
                stpAudio.Visibility = Visibility.Hidden;
                txbAcceptIcon.Text = "\xe6cb";
            }
            else
            {
                txbCallType.Text = ClassHelper.FindResource<string>("IncomingVideoCall");
                txbAcceptIcon.Text = "\xe772";
            }
            txbNickName.SetBinding(TextBlock.TextProperty, new Binding { Source = viewModel.FriendDetails, Path = new PropertyPath(string.IsNullOrEmpty(viewModel.FriendDetails.RemarkName) ? nameof(viewModel.FriendDetails.NickName) : nameof(viewModel.FriendDetails.RemarkName)) });
            imgHeadImage.DataContext = viewModel.FriendDetails.HeadPortrait;
        }

        private async void InviteCallMain_Loaded(object sender, RoutedEventArgs e)
        {
            List<DisplayInfoModel> displayInfos = ClassHelper.GetDisplayInfos();
            if (displayInfos.FirstOrDefault(item => item.MainDisplay) is DisplayInfoModel displayInfo)
            {
                Top = displayInfo.ShowTop + SystemParameters.WorkArea.Height - Height - 5;
                Left = displayInfo.ShowLeft + SystemParameters.WorkArea.Width - Width - 5;
            }

            showMain.Begin(grdMain);
            await Task.Delay(600);
            showInfo.Begin(brdInfo);
            showIcon.Begin(txbAudioIcon);
            showHint.Begin(txbAudio);
            showIcon.Begin(txbDeclineIcon);
            showHint.Begin(txbDecline);
            showIcon.Begin(txbAcceptIcon);
            showHint.Begin(txbAccept);
        }

        // 接受邀请
        private void GrbAccept_PointerUp(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(JoinRoom);
            CloseAnimation();
        }

        #region 执行事件
        private async void CloseAnimation()
        {
            hideInfo.Begin(brdInfo);
            hideIcon.Begin(txbAudioIcon);
            hideHint.Begin(txbAudio);
            hideIcon.Begin(txbDeclineIcon);
            hideHint.Begin(txbDecline);
            hideIcon.Begin(txbAcceptIcon);
            hideHint.Begin(txbAccept);
            await Task.Delay(1000);
            hideMain.Begin(grdMain);
            await Task.Delay(1100);
            Close();
        }
        private void JoinRoom(object data)
        {
            ClassHelper.ShowMask(true);

            if (CallService.ReplyCall(mainData.RoomID, true))
            {
                if (CallService.GetRoomKey(mainData.RoomID, out GetRoomKeyModel roomKey) && CallService.GetRoomMember(mainData.RoomID, out List<string> member))
                {
                    if (ClassHelper.CreatingCallManagement(mainData.RoomID, roomKey, mainData.CallType, member, false))
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
