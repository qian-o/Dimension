using DimensionClient.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DimensionClient.Library.Converters
{
    public class PersonalDetailsConvert : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int gender = System.Convert.ToInt32(values[0], ClassHelper.cultureInfo);
            DateTime? birthday = values[1] as DateTime?;
            string data = string.Empty;
            if (gender != 0)
            {
                data += $"{(gender == 1 ? ClassHelper.FindResource<string>("Boy") : ClassHelper.FindResource<string>("Girl"))} ";
            }
            if (birthday != null)
            {
                data += $"{birthday.Value.GetAge()} {birthday.Value.GetConstellation()}";
            }
            return data;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();

        }
    }
}
