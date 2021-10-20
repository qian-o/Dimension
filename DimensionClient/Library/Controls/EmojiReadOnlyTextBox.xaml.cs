using System.Windows;
using System.Windows.Controls;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// EmojiReadOnlyTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class EmojiReadOnlyTextBox : UserControl
    {
        public string TextContent
        {
            get => (string)GetValue(TextContentProperty);
            set => SetValue(TextContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextContentProperty =
            DependencyProperty.Register("TextContent", typeof(string), typeof(EmojiReadOnlyTextBox), new PropertyMetadata(string.Empty, OnTextContentChanged));

        public static void OnTextContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EmojiReadOnlyTextBox emoji = d as EmojiReadOnlyTextBox;
            emoji.rtbContent.Text = e.NewValue.ToString();
        }

        public EmojiReadOnlyTextBox()
        {
            InitializeComponent();
        }

        private void RtbContent_Loaded(object sender, RoutedEventArgs e)
        {
            rtbContent.Document.PagePadding = new Thickness(0);
        }
    }
}
