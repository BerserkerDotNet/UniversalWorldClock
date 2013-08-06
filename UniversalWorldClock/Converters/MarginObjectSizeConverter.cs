using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalWorldClock.Converters
{
    public sealed class MarginObjectSizeConverter : ObjectSizeConverter
    {
        protected override object Convert(object value, object parameter)
        {
            var size = (double) value;
            return new Thickness(Convert(size));
        }
    }
}