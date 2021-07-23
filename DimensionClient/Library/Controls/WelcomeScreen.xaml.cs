using DimensionClient.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// WelcomeScreen.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomeScreen : UserControl
    {
        public WelcomeScreen()
        {
            InitializeComponent();

            DataContext = ClassHelper.commonView;
            imgHead.SetBinding(Image.SourceProperty, new Binding { Path = new PropertyPath("HeadPortrait"), Converter = ClassHelper.FindResource<IValueConverter>("ImageSourceOnlineConvert"), ConverterParameter = "180" });
            brdOnLine.SetBinding(Border.BackgroundProperty, new Binding { Path = new PropertyPath("OnLine"), Converter = ClassHelper.FindResource<IValueConverter>("OnLineStatusConvert") });
            txbNickName.SetBinding(TextBlock.TextProperty, new Binding { Path = new PropertyPath("NickName") });
        }
    }
}
