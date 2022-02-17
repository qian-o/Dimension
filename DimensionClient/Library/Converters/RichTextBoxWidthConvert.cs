using System.Globalization;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class RichTextBoxWidthConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double originalWidth = System.Convert.ToDouble(value);
            double width = Math.Round(originalWidth, 2);
            if (originalWidth > width)
            {
                width += 0.01;
            }
            return width;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
