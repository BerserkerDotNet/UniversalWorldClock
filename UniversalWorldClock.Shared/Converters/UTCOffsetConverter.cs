using System;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class UTCOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is TimeSpan))
                return value;
            var offset = (TimeSpan) value;
            return string.Format("{0}{1:00}:{2:00} UTC", offset < TimeSpan.Zero ? string.Empty : "+", offset.Hours, offset.Minutes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}