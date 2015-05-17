using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Domain;

namespace UniversalWorldClock.ViewModels.Interfaces
{
    public interface ICitySelectorViewModel
    {
        ObservableCollection<CityInfo> Cities { get; set; }
        ICommand ApplyFilter { get; set; }
        ICommand AddCity { get; set; }
        string Query { get; set; }
    }
}