using Windows.UI.Xaml;

namespace UniversalWorldClock.Converters
{
    public sealed class MarginObjectSizeConverter : ObjectSizeConverter
    {
        protected override object Convert(object value)
        {
            var size = (double) value;
            return new Thickness(Convert(size));
        }
    }
}