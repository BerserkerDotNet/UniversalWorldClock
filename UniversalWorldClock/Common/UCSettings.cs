using System;
using System.Linq;
using Windows.Storage;
using Windows.System.UserProfile;

namespace UniversalWorldClock.Common
{
    public class UCSettings
    {
        public const string TWELVE_HOUR_CLOCK = "12h";
        public const string TWENTY_FOUR_CLOCK = "24h";
        
        private const string CLOCKFORMAT = "ClockFormat";
        private const string CLOCKSIZE = "ClockSize";
        private const string DEFAULT_CLOCK_SIZE = "Small";
        private static ApplicationDataContainer SettingsContainer { get; set; }
        private static string _currentClockSize;
        private static ClockFormat _currentClockFormat;


        static UCSettings()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings;

            _currentClockSize = GetClockSizeOrDefault();
            _currentClockFormat = GetClockFormatOrDefault();
        }

        private static ClockFormat GetClockFormatOrDefault()
        {
            var rawValue = (string)SettingsContainer.Values[CLOCKFORMAT];
            if(!string.IsNullOrEmpty(rawValue))
            {
                return rawValue == TWELVE_HOUR_CLOCK ? ClockFormat.TwelveHourClock : ClockFormat.TwentyFourClock;
            }

            var systemClockFormat = GlobalizationPreferences.Clocks.First();
            var clockFormat = systemClockFormat == "12HourClock" ? ClockFormat.TwelveHourClock : ClockFormat.TwentyFourClock;
            SettingsContainer.Values[CLOCKFORMAT] = clockFormat == ClockFormat.TwelveHourClock?TWELVE_HOUR_CLOCK:TWENTY_FOUR_CLOCK;
            return clockFormat;
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
        
        public static ClockFormat ClockFormat
        {
            get { return _currentClockFormat; }
            set
            {
                if (_currentClockFormat!=value)
                {
                    _currentClockFormat = value;
                    SettingsContainer.Values[CLOCKFORMAT] = value == ClockFormat.TwelveHourClock ? TWELVE_HOUR_CLOCK : TWENTY_FOUR_CLOCK;
                }
            }
        }

    }
}
