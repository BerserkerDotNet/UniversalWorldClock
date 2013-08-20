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
        private IEnumerable<int> _hours;
        private IEnumerable<int> _minutes;
        private int _selectedHours;
        private int _selectedMinutes;
        private ClockInfo _selectedClock;
        private string _timeModifier;
        private IEnumerable<string> _timeModifiers;
        private bool _isShiftUpdateSuppresed = false;

        public TimeSetterViewModel(TimeShiftProvider timeShiftProvider)
        {
            _timeShiftProvider = timeShiftProvider;
            Hours = Enumerable.Range(0, 24).ToList();
            Minutes = Enumerable.Range(0, 60).ToList();
            _timeModifiers = new[] {"AM", "PM"};
            TimeModifier = _timeModifiers.First();
            _timeShiftProvider.TimeShiftCleared += _timeShiftProvider_TimeShiftCleared;
        }

        void _timeShiftProvider_TimeShiftCleared(object sender, EventArgs e)
        {
            _isShiftUpdateSuppresed = true;
            SelectedHours = 0;
            SelectedMinutes = 0;
            _isShiftUpdateSuppresed = false;
        }

        public IEnumerable<int> Hours
        {
            get { return _hours; }
            private set
            {
                _hours = value;
                OnPropertyChanged();
            }
        }    
        public IEnumerable<int> Minutes
        {
            get { return _minutes; }
            private set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> TimeModifiers
        {
            get { return _timeModifiers; }
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

        public int SelectedHours
        {
            get { return _selectedHours; }
            set
            {

                _selectedHours = value;
                OnPropertyChanged();
                UpdateTimeShift();
            }
        }

        public int SelectedMinutes
        {
            get { return _selectedMinutes; }
            set { _selectedMinutes = value;
                OnPropertyChanged();
                UpdateTimeShift();
            }
        }

        public string TimeModifier
        {
            get { return _timeModifier; }
            set { _timeModifier = value; OnPropertyChanged();
                UpdateTimeShift();
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
            var timeShift = GetSelectedTime() - currentTime;

            return new TimeSpan(timeShift.Hours, timeShift.Minutes, 0);
        }

        private TimeSpan GetSelectedTime()
        {
            var hours = SelectedHours;
            if (IsTimeModifierVisible == Visibility.Visible)
            {
                if(TimeModifier == "AM" && SelectedHours==12)
                    hours = 0;
                if (TimeModifier == "PM" && SelectedHours!=12)
                    hours += 12;
            }

            return new TimeSpan(hours, SelectedMinutes, 0);
        }
    }
}