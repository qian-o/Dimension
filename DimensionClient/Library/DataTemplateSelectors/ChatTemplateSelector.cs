using DimensionClient.Common;
using DimensionClient.Models.ResultModels;
using System.Windows;
using System.Windows.Controls;

namespace DimensionClient.Library.DataTemplateSelectors
{
    public class ChatTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChatMessagesModel chatMessages = item as ChatMessagesModel;
            return ClassHelper.FindResource<DataTemplate>(chatMessages.SenderID == ClassHelper.UserID ? "OwnTemplate" : "FriendTemplate");
        }
    }
}
