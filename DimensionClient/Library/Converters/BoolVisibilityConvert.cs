using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class BoolVisibilityConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = System.Convert.ToBoolean(value);
            if (parameter != null && System.Convert.ToInt32(parameter) == 0)
            {
                state = !state;
            }
            return state ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
