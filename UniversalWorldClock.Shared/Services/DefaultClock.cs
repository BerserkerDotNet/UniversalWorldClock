using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using TimeZones;
using UniversalWorldClock.Common;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Services
{
    public class DefaultClock : IClock
    {
        private readonly TimeShiftProvider _timeShiftProvider;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly List<IClockListner> _listners = new List<IClockListner>(10);
        private bool _isInTimeShiftMode = false;

        private readonly Dictionary<string, ITimeZoneEx> _timeZoneServiceCache =
            new Dictionary<string, ITimeZoneEx>();

        public DefaultClock(TimeShiftProvider timeShiftProvider)
        {
            _timeShiftProvider = timeShiftProvider;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
            _timer.Tick += OnTick;
            _timeShiftProvider.TimeShiftStateChanged += TimeShiftProviderTimeShiftStateChanged;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        public void Register(IClockListner listner)
        {
            if (listner == null)
                throw new ArgumentNullException("listner");

            var cityInfo = listner.CityInfo;
            if (cityInfo == null)
                throw new ArgumentException("CityInfo is null");

            _listners.Add(listner);
            if (!_timeZoneServiceCache.ContainsKey(cityInfo.TimeZoneId))
            {
                var service = TimeZoneService.FindSystemTimeZoneById(cityInfo.TimeZoneId);
                _timeZoneServiceCache.Add(cityInfo.TimeZoneId, service);
            }

            NotifyListner(listner);
        }

        public void UnRegister(IClockListner listner)
        {
            _listners.Remove(listner);
        }

        public TimeSpan GetOffset(string timeZoneId)
        {
            var timeZoneEx = GetTimeService(timeZoneId);
            return timeZoneEx.ConvertTime(DateTime.Now).Offset;
        }

        public DateTime ConvertTime(string timeZoneId, DateTime time)
        {
            var timeZoneEx = GetTimeService(timeZoneId);
            return timeZoneEx.ConvertTime(time).DateTime;
        }

        private DateTime CalculateTime(ITimeZoneEx timeService)
        {
            var dateTimeOffset = timeService.ConvertTime(DateTime.Now);
            return dateTimeOffset.DateTime + _timeShiftProvider.TimeShift;
        }

        private void OnTick(object sender, object e)
        {
            NotifyListners();
            var bounds = Window.Current.Bounds;
            SetTimerInterval(bounds.Width);
        }

        private void NotifyListners()
        {
            foreach (var clockListner in _listners)
            {
                NotifyListner(clockListner);
            }

        }

        private void NotifyListner(IClockListner clockListner)
        {
            var service = GetTimeService(clockListner.CityInfo.TimeZoneId);
            var date = CalculateTime(service);
            clockListner.TickTack(date);
        }

        private ITimeZoneEx GetTimeService(string timeZoneId)
        {
            if (_timeZoneServiceCache.ContainsKey(timeZoneId))
                return _timeZoneServiceCache[timeZoneId];

            var service = TimeZoneService.FindSystemTimeZoneById(timeZoneId);
            _timeZoneServiceCache.Add(timeZoneId, service);
            return service;

        }

        private void SetTimerInterval(double windowWidth)
        {
            _timer.Interval = CalculateTimerInterval(ViewStateHelper.GetViewState(windowWidth));
        }

        private TimeSpan CalculateTimerInterval(ViewState state)
        {
            if (_isInTimeShiftMode)
                return TimeSpan.FromSeconds(1);

            if (state == ViewState.FullScreenLandscape)
                return TimeSpan.FromSeconds(1);

            if ((state == ViewState.Snapped || state == ViewState.Narrow) && DateTime.Now.Second == 0)
            {
                return TimeSpan.FromMinutes(1);
            }

            if ((state == ViewState.Snapped || state == ViewState.Narrow) && DateTime.Now.Second != 0)
            {
                var interval = (60 - DateTime.Now.Second);
                return TimeSpan.FromSeconds(interval);
            }

            return TimeSpan.FromSeconds(1);
        }

        private void TimeShiftProviderTimeShiftStateChanged(object sender, TimeShiftStateChangedArgs e)
        {
            _isInTimeShiftMode = e.IsStarted;
            SetTimerInterval(Window.Current.Bounds.Width);
        }

        //TODO: Smels bad
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetTimerInterval(e.Size.Width);
        }
    }
}