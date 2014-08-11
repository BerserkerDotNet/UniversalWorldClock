using System;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class DateToShortStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var d = (DateTime) value;
            return d.ToString("dd MMM");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}