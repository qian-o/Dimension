using DimensionClient.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class PopupMaskConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value, ClassHelper.cultureInfo) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
