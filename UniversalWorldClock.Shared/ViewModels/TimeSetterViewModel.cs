using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using TimeZones;
using UniversalWorldClock.Common;
using UniversalWorldClock.Data;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.ViewModels
{
    public sealed class TimeSetterViewModel : ViewModel
    {
        private readonly TimeShiftProvider _timeShiftProvider;
        private readonly IDataRepository _dataRepository;
        private CityInfo _selectedClock;
        private bool _isShiftUpdateSuppresed;
        private TimeSpan _selectedTime;

        public TimeSetterViewModel(TimeShiftProvider timeShiftProvider, IDataRepository dataRepository)
        {
            _timeShiftProvider = timeShiftProvider;
            _dataRepository = dataRepository;
            _timeShiftProvider.TimeShiftCleared += _timeShiftProvider_TimeShiftCleared;
        }

        void _timeShiftProvider_TimeShiftCleared(object sender, EventArgs e)
        {
            _isShiftUpdateSuppresed = true;
            SelectedTime = TimeSpan.Zero;
            _isShiftUpdateSuppresed = false;
        }

        public CityInfo SelectedClock
        {
            get { return _selectedClock; }
            set
            {
                _selectedClock = value;
                OnPropertyChanged(() => SelectedClock);
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
                    OnPropertyChanged(() => SelectedTime);
                }
            }
        }

        public IEnumerable<CityInfo> Clocks
        {
            get { return _dataRepository.GetUsersCities(); }
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