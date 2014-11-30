using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public sealed class InversedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isVisible = (bool) value;
            return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}