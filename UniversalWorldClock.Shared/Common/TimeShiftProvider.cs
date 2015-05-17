using System;
using System.ComponentModel;

namespace UniversalWorldClock.Common
{
    public class TimeShiftProvider : INotifyPropertyChanged
    {
        private TimeSpan _timeShift;

        public void Clear()
        {
            OnTimeShiftCleared(EventArgs.Empty);
            TimeShift = TimeSpan.Zero;
        }

        public TimeSpan TimeShift
        {
            get { return _timeShift; }
            set
            {
                _timeShift = value;
                OnPropertyChanged("TimeShift");
            }
        }

        public void StartTimeShift()
        {
            OnTimeShiftStateChanged(new TimeShiftStateChangedArgs(true));
        }
        
        public void EndTimeShift()
        {
            OnTimeShiftStateChanged(new TimeShiftStateChangedArgs(false));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnTimeShiftCleared(EventArgs e)
        {
            EventHandler<EventArgs> handler = TimeShiftCleared;
            if (handler != null) handler(this, e);
        }

        protected void OnTimeShiftStateChanged(TimeShiftStateChangedArgs e)
        {
            var handler = TimeShiftStateChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<TimeShiftStateChangedArgs> TimeShiftStateChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<EventArgs> TimeShiftCleared;
    }
}