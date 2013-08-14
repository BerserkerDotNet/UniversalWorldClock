using System;
using System.Diagnostics;
using UniversalWorldClock.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UniversalWorldClock.Converters
{
    public class ObjectSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return Convert(value, parameter);
            }
            catch (Exception)
            {
                return value;
            }
        }

        protected virtual object Convert(object value, object parameter)
        {
            var size = (double)value;
            return Convert(size);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        public double Convert(double value)
        {
            var mult = GetMultiplier();
            return value * mult;
        }

        private double GetMultiplier()
        { 
            //Note: performance issue, try to cache value
            switch (UCSettings.ClockSize.ToLower())
            {
                case "small":
                    return 1.0;
                case "medium":
                    return 1.5;
                case "large":
                    return 2.0;
                default:
                    return 1.0;
            }
      
            
        }
    }
}