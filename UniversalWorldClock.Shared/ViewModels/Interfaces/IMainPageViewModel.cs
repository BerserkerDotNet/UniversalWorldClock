using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.ViewModels.Interfaces
{
    public interface IMainPageViewModel
    {
        ICommand Add { get; }
        ICommand SetTime { get; }
        ObservableCollection<CityInfo> Clocks { get; set; }
        ICommand Donate { get; }
        bool MenuActive { get; }
        ICommand SetShowMenu { get; }
        ICommand GoToSettings { get; }
        ICommand GoToDetails { get; }
    }
}