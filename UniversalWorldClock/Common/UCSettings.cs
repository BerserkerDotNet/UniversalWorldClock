using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UniversalWorldClock.Common
{
    public class UCSettings
    {
        private const string CLOCKFORMAT = "ClockFormat";
        private const string CLOCKSIZE = "ClockSize";
        private const string DEFAULT_CLOCK_SIZE = "Small";
        private const string DEFAULT_CLOCK_FORMAT = "12h";
        private static ApplicationDataContainer SettingsContainer { get; set; }
        private static string _currentClockSize;
        private static string _currentClockFormat;


        static UCSettings()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings;

            _currentClockSize = GetClockSizeOrDefault();
            _currentClockFormat = GetClockFormatOrDefault();
        }

        private static string GetClockFormatOrDefault()
        {
            var value = SettingsContainer.Values[CLOCKFORMAT] as string;
            if (string.IsNullOrEmpty(value))
            {
                SettingsContainer.Values[CLOCKFORMAT] = DEFAULT_CLOCK_FORMAT;
                value = DEFAULT_CLOCK_FORMAT;
            }
            return value;
        }

        private static string GetClockSizeOrDefault()
        {
            var value = SettingsContainer.Values[CLOCKSIZE] as string;
            if (string.IsNullOrEmpty(value))
            {
                SettingsContainer.Values[CLOCKSIZE] = DEFAULT_CLOCK_SIZE;
                value = DEFAULT_CLOCK_SIZE;
            }
            return value;
        }

        public static string ClockSize
        {
            get { return _currentClockSize; }
            set
            {
                if (!_currentClockSize.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    _currentClockSize = value;
                    SettingsContainer.Values[CLOCKSIZE] = value;
                }
            }
        }
        
        public static string ClockFormat
        {
            get { return _currentClockFormat; }
            set
            {
                if (!_currentClockFormat.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    _currentClockFormat = value;
                    SettingsContainer.Values[CLOCKFORMAT] = value;
                }
            }
        }

    }
}
