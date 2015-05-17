using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Mvvm;
using UniversalWorldClock.Common;
using UniversalWorldClock.Runtime;

namespace UniversalWorldClock.ViewModels
{
    public sealed class TimeShifterViewModel : ViewModel
    {
        private readonly TimeShiftProvider _timeShiftProvider;
        private TimeSpan _globalTimeShift;
        private bool _isTimeShiftSuppresed;

        public TimeShifterViewModel(TimeShiftProvider timeShiftProvider)
        {
            _timeShiftProvider = timeShiftProvider;
            _timeShiftProvider.TimeShiftCleared += _timeShiftProvider_TimeShiftCleared;
            StartShift = new RelayCommand(() => { _timeShiftProvider.StartTimeShift(); });
            EndShift = new RelayCommand(() => { _timeShiftProvider.EndTimeShift(); });
        }

        private void _timeShiftProvider_TimeShiftCleared(object sender, EventArgs e)
        {
            _isTimeShiftSuppresed = true;
            GlobalHourTimeShift = 0;
            GlobalMinuteTimeShift = 0;
            _isTimeShiftSuppresed = false;
        }

        private void UpdateTimeShift()
        {
            if (!_isTimeShiftSuppresed)
                _timeShiftProvider.TimeShift = _globalTimeShift;

        }

        public int GlobalHourTimeShift
        {
            get { return _globalTimeShift.Hours; }
            set
            {
                var isHoursBecomesNegative = (value < 0 && _globalTimeShift.Minutes > 0);
                var isHoursBecomesPositiveOrZero = (value >= 0 && _globalTimeShift.Minutes < 0);
                var minutes = isHoursBecomesNegative || isHoursBecomesPositiveOrZero
                    ? -_globalTimeShift.Minutes
                    : _globalTimeShift.Minutes;

                _globalTimeShift = new TimeSpan(0, value, minutes, 0);
                OnPropertyChanged(() => GlobalHourTimeShift);
                UpdateTimeShift();
            }
        }

        public int GlobalMinuteTimeShift
        {
            get { return _globalTimeShift.Minutes; }
            set
            {
                var minutes = _globalTimeShift.Hours < 0 ? -value : value;
                _globalTimeShift = new TimeSpan(0, _globalTimeShift.Hours, minutes, 0);
                OnPropertyChanged(() => GlobalMinuteTimeShift);
                UpdateTimeShift();
            }
        }

        public ICommand StartShift { get; set; }
        public ICommand EndShift { get; set; }
    }
}