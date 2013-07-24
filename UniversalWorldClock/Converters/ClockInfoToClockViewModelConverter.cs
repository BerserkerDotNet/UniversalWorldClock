using System;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class ClockInfoToClockViewModelConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var info = value as ClockInfo;
            if (info == null)
                throw new ArgumentException("Use this converter on proper type");
            return new ClockViewModel(info);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
