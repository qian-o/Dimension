using System.Windows;
using System.Windows.Controls;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ToastMessage.xaml 的交互逻辑
    /// </summary>
    public partial class ToastMessage : UserControl
    {
        private readonly string _message;
        private readonly int _messageType;

        public ToastMessage(string message, int messageType = 0)
        {
            _message = message;
            _messageType = messageType is >= 0 and <= 3 ? messageType : 2;

            InitializeComponent();
        }

        private void UserControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            ResourceDictionary dictionary = (Application.Current.Resources["MessageType"] as ResourceDictionary).MergedDictionaries[_messageType];
            Resources.MergedDictionaries.Add(dictionary);
            txbMessage.Text = _message;
        }
    }
}
