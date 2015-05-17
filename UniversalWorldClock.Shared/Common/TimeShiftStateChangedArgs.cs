using System;

namespace UniversalWorldClock.Common
{
    public class TimeShiftStateChangedArgs : EventArgs
    {
        public TimeShiftStateChangedArgs(bool isStarted)
        {
            IsStarted = isStarted;
        }

        public bool IsStarted { get; private set; }
    }
}