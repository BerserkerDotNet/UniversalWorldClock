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
        public static ApplicationDataContainer SettingsContainer { get; set; }

        static UCSettings()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings;
        }

        public static string ClockSize
        {
            get
            {
                var value = SettingsContainer.Values[CLOCKSIZE] as string;
                if (string.IsNullOrEmpty(value))
                {
                    SettingsContainer.Values[CLOCKSIZE] = DEFAULT_CLOCK_SIZE;
                    value = DEFAULT_CLOCK_SIZE;
                }
                return value;
            }
            set { SettingsContainer.Values[CLOCKSIZE] = value; }
        }
        
        public static string ClockFormat
        {
            get
            {
                var value = SettingsContainer.Values[CLOCKFORMAT] as string;
                if(string.IsNullOrEmpty(value))
                {
                    SettingsContainer.Values[CLOCKFORMAT] = DEFAULT_CLOCK_FORMAT;
                    value = DEFAULT_CLOCK_FORMAT;
                }
                return value;
            }
            set { SettingsContainer.Values[CLOCKFORMAT] = value; }
        }

    }
}
