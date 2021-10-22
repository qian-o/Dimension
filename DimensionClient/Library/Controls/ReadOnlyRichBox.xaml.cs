using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace DimensionClient.Library.Controls
{
    /// <summary>
    /// ReadOnlyRichBox.xaml 的交互逻辑
    /// </summary>
    public partial class ReadOnlyRichBox : UserControl
    {
        public string TextContent
        {
            get => (string)GetValue(TextContentProperty);
            set => SetValue(TextContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextContentProperty =
            DependencyProperty.Register("TextContent", typeof(string), typeof(ReadOnlyRichBox), new PropertyMetadata(string.Empty, OnTextContentChanged));

        public static void OnTextContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReadOnlyRichBox onlyRichText = d as ReadOnlyRichBox;
            onlyRichText.txbRichBox.Text = e.NewValue.ToString();
            onlyRichText.rtbRichBox.Document.Blocks.Clear();
            _ = new Run(e.NewValue.ToString(), onlyRichText.rtbRichBox.Document.ContentEnd);
        }

        public string SerializedContent
        {
            get => (string)GetValue(SerializedContentProperty);
            set => SetValue(SerializedContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for SerializedContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SerializedContentProperty =
            DependencyProperty.Register("SerializedContent", typeof(string), typeof(ReadOnlyRichBox), new PropertyMetadata(string.Empty, OnSerializedContentChanged));

        public static void OnSerializedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReadOnlyRichBox onlyRichText = d as ReadOnlyRichBox;
            onlyRichText.rtbRichBox.Document = XamlReader.Parse(e.NewValue.ToString()) as FlowDocument;
            Block[] blocks = new Block[onlyRichText.rtbRichBox.Document.Blocks.Count];
            (XamlReader.Parse(e.NewValue.ToString()) as FlowDocument).Blocks.CopyTo(blocks, 0);
            foreach (Block item in blocks)
            {
                if (item is Paragraph paragraph)
                {
                    if (item != blocks[0])
                    {
                        onlyRichText.txbRichBox.Inlines.Add(Environment.NewLine);
                    }
                    Inline[] inlines = new Inline[paragraph.Inlines.Count];
                    paragraph.Inlines.CopyTo(inlines, 0);
                    onlyRichText.txbRichBox.Inlines.AddRange(inlines);
                }
            }
        }

        public ReadOnlyRichBox()
        {
            InitializeComponent();
        }

        private void RtbRichBox_Loaded(object sender, RoutedEventArgs e)
        {
            rtbRichBox.Document.PagePadding = new Thickness(0);
        }
    }
}
