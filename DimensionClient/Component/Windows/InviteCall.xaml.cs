using DimensionClient.Common;
using DimensionClient.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DimensionClient.Component.Windows
{
    /// <summary>
    /// InviteCall.xaml 的交互逻辑
    /// </summary>
    public partial class InviteCall : Window
    {
        private readonly Storyboard showMain = ClassHelper.FindResource<Storyboard>("InviteCallShowMain");
        private readonly Storyboard showInfo = ClassHelper.FindResource<Storyboard>("InviteCallShowInfo");
        private readonly Storyboard showIcon = ClassHelper.FindResource<Storyboard>("InviteCallShowIcon");
        private readonly Storyboard showHint = ClassHelper.FindResource<Storyboard>("InviteCallShowHint");
        private readonly Storyboard hideMain = ClassHelper.FindResource<Storyboard>("InviteCallHideMain");
        private readonly Storyboard hideInfo = ClassHelper.FindResource<Storyboard>("InviteCallHideInfo");
        private readonly Storyboard hideIcon = ClassHelper.FindResource<Storyboard>("InviteCallHideIcon");
        private readonly Storyboard hideHint = ClassHelper.FindResource<Storyboard>("InviteCallHideHint");

        public InviteCall()
        {
            InitializeComponent();
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
            showIcon.Begin(txbVideoIcon);
            showHint.Begin(txbVideo);
        }

        #region 接受邀请(鼠标,触控)
        private void GrbAccept_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                GrbAccept_PointerUp();
            }
        }
        private void GrbAccept_TouchUp(object sender, TouchEventArgs e)
        {
            GrbAccept_PointerUp();
        }
        #endregion

        #region 执行事件
        private void GrbAccept_PointerUp()
        {

            CloseAnimation();
        }
        private async void CloseAnimation()
        {
            hideInfo.Begin(brdInfo);
            hideIcon.Begin(txbAudioIcon);
            hideHint.Begin(txbAudio);
            hideIcon.Begin(txbDeclineIcon);
            hideHint.Begin(txbDecline);
            hideIcon.Begin(txbVideoIcon);
            hideHint.Begin(txbVideo);
            await Task.Delay(1000);
            hideMain.Begin(grdMain);
            await Task.Delay(1100);
            Close();
        }
        #endregion
    }
}
