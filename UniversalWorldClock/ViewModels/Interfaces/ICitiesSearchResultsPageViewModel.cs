using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversalWorldClock.Models;

namespace UniversalWorldClock.ViewModels.Interfaces
{
    public interface ICitiesSearchResultsPageViewModel
    {
        ObservableCollection<SearchFilter> Filters { get; set; }
        ObservableCollection<SearchResult> Results { get; set; }
        bool ShowFilters { get; set; }
        bool IsInProgress { get; set; }
        string QueryText { get; set; }
        SearchFilter CurrentFilter { get; set; }
        ICommand ApplyFilter { get; set; }
        ICommand SelectFilter { get; set; }
        ICommand AddCity { get; set; }
    }
}