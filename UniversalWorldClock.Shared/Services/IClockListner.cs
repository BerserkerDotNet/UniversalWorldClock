using System;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.Services
{
    public interface IClockListner
    {
        CityInfo CityInfo { get; }
        void TickTack(DateTime date);
    }
}