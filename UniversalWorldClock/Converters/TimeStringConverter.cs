using System;
using UniversalWorldClock.Common;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            var clockFormat = UCSettings.ClockFormat;
            var format = clockFormat != null && clockFormat.Equals("24h",StringComparison.OrdinalIgnoreCase)
                             ? "{0:HH\\:mm}"
                             : (parameter == null) ? "{0:hh\\:mm tt}" : "{0:hh\\:mm}";

            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}