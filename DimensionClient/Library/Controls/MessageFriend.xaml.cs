using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using System.Windows;
using System.Windows.Controls;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// MessageFriend.xaml 的交互逻辑
    /// </summary>
    public partial class MessageFriend : UserControl
    {
        public MessageFriend()
        {
            InitializeComponent();
        }

        private void UserControlMain_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ChatMessagesModel chatMessages)
            {
                imgHead.DataContext = chatMessages.SenderID;
                txbTime.Text = chatMessages.CreateTime.ToString("t", ClassHelper.cultureInfo);
                txbContent.Text = chatMessages.MessageContent;
            }
        }
    }
}
