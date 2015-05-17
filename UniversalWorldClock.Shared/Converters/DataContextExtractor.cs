using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public class DataContextExtractor:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var element = parameter as FrameworkElement;
            if (element == null)
                return null;

            return element.DataContext;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}