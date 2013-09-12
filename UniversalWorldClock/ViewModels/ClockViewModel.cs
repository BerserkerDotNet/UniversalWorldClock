using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using TimeZones;
using UniversalWorldClock.Common;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using UniversalWorldClock.Views;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UniversalWorldClock.ViewModels
{
    public sealed class ClockViewModel : ViewModelBase
    {
        private readonly ClockInfo _info;
        private readonly TimeShiftProvider _timeShiftProvider;
        private ITimeZoneEx _timeZoneService;
        private static readonly DispatcherTimer _timer = new DispatcherTimer();
        private DateTime _date;

        static ClockViewModel()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
            _timer.Tick += ViewStateTracker;
        }

        public ClockViewModel(ClockInfo info, TimeShiftProvider timeShiftProvider)
        {
            _info = info;
            _timeShiftProvider = timeShiftProvider;
            _timeZoneService = TimeZoneService.FindSystemTimeZoneById(_info.TimeZoneId);
            _timer.Tick += OnTimerTick;
            Delete = new RelayCommand(() => ViewModelStorage.Main.DeleteClock(_info)); //Clean-up
            CalculateTime();
            _timeShiftProvider.PropertyChanged += (s, e) => CalculateTime();
        }

        void OnTimerTick(object sender, object e)
        {
            CalculateTime();
        }

        private void CalculateTime()
        {
            var dateTimeOffset = _timeZoneService.ConvertTime(DateTime.Now);
            Date = dateTimeOffset.DateTime + _timeShiftProvider.TimeShift;
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
                if (_date!=value)
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

        //NOTE: There should be no UI types here, consider to use a converter
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

        public void ApplyViewState()
        {
            _timer.Interval = CalculateTimerInterval();
        }

        private static void ViewStateTracker(object sender, object e)
        {
            _timer.Interval = CalculateTimerInterval();
        }

        private static TimeSpan CalculateTimerInterval()
        {
            if (ApplicationView.Value != ApplicationViewState.Snapped)
                return TimeSpan.FromSeconds(1);

            if (ApplicationView.Value == ApplicationViewState.Snapped && DateTime.Now.Second == 0)
            {
                return TimeSpan.FromMinutes(1);
            }

            if (ApplicationView.Value == ApplicationViewState.Snapped && DateTime.Now.Second != 0)
            {
                var interval = (60 - DateTime.Now.Second);
                return TimeSpan.FromSeconds(interval);
            }

            return TimeSpan.FromSeconds(1);
        }

    }
}