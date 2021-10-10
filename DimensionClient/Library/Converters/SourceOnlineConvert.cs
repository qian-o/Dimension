using DimensionClient.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class SourceOnlineConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Uri($"{ClassHelper.servicePath}/api/Attachment/GetAttachments/{value}", UriKind.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
