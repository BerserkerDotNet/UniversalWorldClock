using System;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class TimeToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTime)
            {
                var dt = (DateTime) value;
                switch(parameter.ToString().ToLower())
                {
                    case "hours":
                        return 30 * dt.Hour + dt.Minute / 2 + dt.Second / 120;;
                    case "minutes":
                        return 6 * dt.Minute + dt.Second / 10;
                    case "seconds":
                        return dt.Second * 6;
                    default:
                        return 0;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}