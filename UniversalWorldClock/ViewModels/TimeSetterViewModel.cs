using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeZones;
using UniversalWorldClock.Common;
using UniversalWorldClock.Domain;
using UniversalWorldClock.Runtime;
using Windows.UI.Xaml;

namespace UniversalWorldClock.ViewModels
{
    public sealed class TimeSetterViewModel : ViewModelBase
    {
        private readonly TimeShiftProvider _timeShiftProvider;
        private ClockInfo _selectedClock;
        private bool _isShiftUpdateSuppresed = false;
        private TimeSpan _selectedTime;

        public TimeSetterViewModel(TimeShiftProvider timeShiftProvider)
        {
            _timeShiftProvider = timeShiftProvider;
            _timeShiftProvider.TimeShiftCleared += _timeShiftProvider_TimeShiftCleared;
        }

        void _timeShiftProvider_TimeShiftCleared(object sender, EventArgs e)
        {
            _isShiftUpdateSuppresed = true;
            SelectedTime = TimeSpan.Zero;
            _isShiftUpdateSuppresed = false;
        }

        public ClockInfo SelectedClock
        {
            get { return _selectedClock; }
            set
            {
                _selectedClock = value;
                OnPropertyChanged();
                _timeShiftProvider.TimeShift = CalculateShift();
            }
        }

        public TimeSpan SelectedTime
        {
            get { return _selectedTime; }
            set
            {
                if (_selectedTime != value)
                {
                    _selectedTime = value;
                    UpdateTimeShift();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateTimeShift()
        {
            if(_isShiftUpdateSuppresed)
                return;
            _timeShiftProvider.TimeShift = CalculateShift();
        }

        private TimeSpan CalculateShift()
        {
            if (SelectedClock == null)
                return TimeSpan.Zero;
            var service = TimeZoneService.FindSystemTimeZoneById(SelectedClock.TimeZoneId);
            var currentTime = service.ConvertTime(DateTime.Now).TimeOfDay;
            var timeShift = _selectedTime - currentTime;

            return new TimeSpan(timeShift.Hours, timeShift.Minutes, 0);
        }

        public string ClockIdentifier
        {
            get
            {
                var is12HourClock =  UCSettings.ClockFormat == ClockFormat.TwelveHourClock;
                return is12HourClock ? "12HourClock" : "24HourClock";
            }
        }
    }
}