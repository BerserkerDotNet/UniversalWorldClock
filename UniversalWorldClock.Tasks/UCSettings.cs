using System;
using System.Linq;
using Windows.Storage;
using Windows.System.UserProfile;

namespace UniversalWorldClock.Tasks
{
    public sealed class TaskSettings
    {
        private const string TWELVE_HOUR_CLOCK = "12h";
        
        private const string CLOCKFORMAT = "ClockFormat";

        public static ClockFormat GetClockFormatOrDefault()
        {
            var rawValue = (string)ApplicationData.Current.LocalSettings.Values[CLOCKFORMAT];
            if (!string.IsNullOrEmpty(rawValue))
            {
                return rawValue == TWELVE_HOUR_CLOCK ? ClockFormat.TwelveHourClock : ClockFormat.TwentyFourClock;
            }

            var systemClockFormat = GlobalizationPreferences.Clocks.First();
            var clockFormat = systemClockFormat == "12HourClock"
                ? ClockFormat.TwelveHourClock
                : ClockFormat.TwentyFourClock;
            return clockFormat;
        }


    }
    public enum ClockFormat
    {
        TwelveHourClock,
        TwentyFourClock

    }
}
