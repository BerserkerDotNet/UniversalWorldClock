using System;
using System.Collections.Generic;
using System.Linq;
using UniversalWorldClock.Common;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class HoursArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var hours = value as IEnumerable<int>;

            var enumerable = UCSettings.ClockFormat.Equals("24h") ? hours : hours.TakeWhile(h => h < 13).Skip(1);
            return enumerable;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}