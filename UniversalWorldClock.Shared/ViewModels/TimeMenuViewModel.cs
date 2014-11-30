using System.Windows.Input;
using UniversalWorldClock.Common;
using UniversalWorldClock.Runtime;

namespace UniversalWorldClock.ViewModels
{
    public class TimeMenuViewModel : ViewModelBase
    {
        private readonly TimeShiftProvider _timeShiftProvider;

        public TimeMenuViewModel(TimeShiftProvider timeShiftProvider)
        {
            _timeShiftProvider = timeShiftProvider;
            ClearTimeShift = new RelayCommand(() => _timeShiftProvider.Clear());
        }

        public TimeShiftProvider ShiftProvider
        {
            get { return _timeShiftProvider; }
        }

        public ICommand ClearTimeShift { get; private set; }
    }
}