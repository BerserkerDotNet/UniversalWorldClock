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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<EventArgs> TimeShiftCleared;

        public void OnTimeShiftCleared(EventArgs e)
        {
            EventHandler<EventArgs> handler = TimeShiftCleared;
            if (handler != null) handler(this, e);
        }
    }
}