using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.ViewModels.Interfaces
{
    public interface ICityDetailsPageViewModel
    {
        CityInfo CityInfo { get; set; }
        ObservableCollection<SunInfo> SunInfo { get; }
        string Coordinates { get; }
        DateTime Date { get; set; }
        TimeSpan UTCOffset { get; }
        ObservableCollection<CityInfo> Clocks { get; set; }

        ICommand ApplySelection { get; }
        ICommand Delete { get; }
    }
}