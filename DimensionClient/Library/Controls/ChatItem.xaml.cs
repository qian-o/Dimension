using DimensionClient.Models.ResultModels;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ChatItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatItem : UserControl
    {
        public ChatItem()
        {
            InitializeComponent();
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ChatColumnInfoModel chatColumnInfo)
            {
                imgHead.DataContext = chatColumnInfo.HeadPortrait;
                txbNickName.Text = chatColumnInfo.NickName;
                txbRemarkName.Text = chatColumnInfo.RemarkName;
                ThreadPool.QueueUserWorkItem(Load, chatColumnInfo.ChatID);
            }
        }

        #region 执行事件
        private void Load(object data)
        {

        }
        #endregion
    }
}
