using DimensionClient.Common;
using System.Globalization;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class SourceOnlineConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int height = 0;
            if (parameter != null)
            {
                height = System.Convert.ToInt32(parameter);
            }
            string uri = $"{ClassHelper.servicePath}/api/Attachment/GetAttachments/{value}";
            uri += height != 0 ? $"?height={height}" : string.Empty;
            return new Uri(uri, UriKind.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
