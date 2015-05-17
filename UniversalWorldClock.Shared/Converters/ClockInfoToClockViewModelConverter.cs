using System;
using Windows.UI.Xaml.Data;
using UniversalWorldClock.Domain;
using UniversalWorldClock.ViewModels;

namespace UniversalWorldClock.Converters
{
    public sealed class ClockInfoToClockViewModelConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var info = value as CityInfo;
            if (info == null)
                throw new ArgumentException("Use this converter on proper type");

            return App.Resolve<ClockViewModel>(new Tuple<string, object>("info", info));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
