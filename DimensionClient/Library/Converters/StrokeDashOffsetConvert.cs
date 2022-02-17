using System.Globalization;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class StrokeDashOffsetConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (100 - (int)value) * (int)parameter / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
