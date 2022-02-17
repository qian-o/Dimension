using System.Globalization;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class OnLineStatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? "#67C23A" : "#909399";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
