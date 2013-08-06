using System;
using System.Globalization;
using System.Windows.Input;
using TimeZones;
using UniversalWorldClock.Common;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using Windows.UI.Xaml;

namespace UniversalWorldClock.ViewModels
{
    public sealed class ClockViewModel : ViewModelBase
    {
        private readonly ClockInfo _info;
        private ITimeZoneEx _timeZoneService;
        private static readonly DispatcherTimer _timer = new DispatcherTimer();
        private DateTime _date;

        static ClockViewModel()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
        }

        public ClockViewModel(ClockInfo info)
        {
            _info = info;
            _timeZoneService = TimeZoneService.FindSystemTimeZoneById(_info.TimeZoneId);
            _timer.Tick += OnTimerTick;
            Delete = new RelayCommand(() => ViewModelStorage.Main.DeleteClock(_info));
        }

        void OnTimerTick(object sender, object e)
        {
            var dateTimeOffset = _timeZoneService.ConvertTime(DateTime.Now);
            Date = dateTimeOffset.DateTime + TimeOffset + ViewModelStorage.Main.TimeShift;
        }

        public string CityName
        {
            get { return _info.CityName; }
        } 
        public string CountryName
        {
            get { return _info.CountryName; }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan UTCOffset
        {
            get
            {
                var dateTimeOffset = _timeZoneService.ConvertTime(DateTime.Now);

                return dateTimeOffset.Offset;
            }
        }

        public string CountryCode
        {
            get { return _info.CountryCode; }
        }

        public static TimeSpan TimeOffset { get; set; }
        //NOTE: It should be no UI types here, consider to use a converter
        public Visibility IsTimeModifierVisible
        {
            get
            {
                return (UCSettings.ClockFormat != null &&
                        UCSettings.ClockFormat.Equals("24h", StringComparison.OrdinalIgnoreCase))
                           ? Visibility.Collapsed
                           : Visibility.Visible;
            }
        }

        public ICommand Delete { get; private set; }



    }
}