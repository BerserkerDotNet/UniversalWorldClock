using System;

namespace UniversalWorldClock.Services
{
    public interface IClock
    {
        void Register(IClockListner listner);
        void UnRegister(IClockListner listner);

        TimeSpan GetOffset(string timeZoneId);
        DateTime ConvertTime(string timeZoneId, DateTime time);
    }
}
